using Confluent.Kafka;
using RocksDbSharp;
using System.Diagnostics;
using System.Text.Json;
using Telemetry.Contracts.Constants;
using Telemetry.Contracts.Events;
using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.Logging;
using Telemetry.Ingress.API.Infrastructure.Observability.HighPerformanceLogging;
using Telemetry.Ingress.API.Infrastructure.Observability.Otel;
using Telemetry.Ingress.API.Infrastructure.Services;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class TelemetryPublishWorker(
    LocalBufferService buffer,
    IEventMessageBus messageBus,
    ILogger<TelemetryPublishWorker> logger,
    IngressMetrics metrics)
    : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new(OtelConstants.ActivitySourceName);
    private const int BatchSize = 500;
    public const string PublishActivityName = "Kafka Publish Event";

    private const int MaxDelayMs = 1000 * 30; // 30s

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogStarted();

        int currentDelayMs = 1000;

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    bool hasProcessedData = await TryProcessNextBatchAsync(stoppingToken);

                    currentDelayMs = 1000;

                    if (!hasProcessedData)
                    {
                        await Task.Delay(50, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogProcessingError(nameof(TelemetryPublishWorker), ex);

                    await Task.Delay(currentDelayMs, stoppingToken);

                    currentDelayMs = Math.Min(currentDelayMs * 2, MaxDelayMs);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogCancelled();
        }
    }

    private async Task<bool> TryProcessNextBatchAsync(CancellationToken stoppingToken)
    {
        using var iterator = buffer.NewIterator();
        iterator.SeekToFirst();

        if (!iterator.Valid())
        {
            return false;
        }

        var keysToDelete = new List<byte[]>(BatchSize);
        var publishTasks = new List<Task>(BatchSize);
        using var writeBatch = new WriteBatch();
        bool hasPoisonPills = false;

        while (iterator.Valid() && publishTasks.Count < BatchSize)
        {
            var key = iterator.Key();
            var value = iterator.Value();

            EnvelopedEvent? envelope = null;
            try
            {
                envelope = JsonSerializer.Deserialize<EnvelopedEvent>(value);
            }
            catch (Exception ex)
            {
                logger.LogWorkerDeserializationError(ex);
                metrics.RecordPoisolPill(ex.GetType().Name);

                writeBatch.Delete(key);
                hasPoisonPills = true;

                iterator.Next();
                continue;
            }

            if (envelope == null)
            {
                writeBatch.Delete(key);
                hasPoisonPills = true;

                iterator.Next();
                continue;
            }

            publishTasks.Add(PublishWithTracingAsync(envelope, stoppingToken));
            keysToDelete.Add(key);

            iterator.Next();
        }

        if (publishTasks.Count == 0 && hasPoisonPills)
        {
            buffer.Write(writeBatch);
            return true;
        }

        await Task.WhenAll(publishTasks);

        foreach (var key in keysToDelete)
        {
            writeBatch.Delete(key);
        }

        buffer.Write(writeBatch);

        return true;
    }

    private async Task PublishWithTracingAsync(EnvelopedEvent envelope, CancellationToken stoppingToken)
    {
        var parentContext = string.IsNullOrEmpty(envelope.TraceParent)
            ? default
            : ActivityContext.Parse(envelope.TraceParent, null);

        using var activity = ActivitySource.StartActivity(
            PublishActivityName,
            ActivityKind.Producer,
            parentContext);

        activity?.SetTag(OtelTagConstants.MessagingSystem, "kafka");

        try
        {
            await messageBus.PublishAsync(envelope.Payload, parentContext, stoppingToken);
        }
        catch (ProduceException<string, string> ex)
        {
            // retry if recoverable
            if (ex.Error.IsFatal || ex.Error.Code == ErrorCode.Local_MsgTimedOut || ex.Error.IsBrokerError)
            {
                throw;
            }

            logger.LogKafkaMessageRejected(envelope.Payload.EventId, ex);

            metrics.RecordPermanentRejection(ex.Error.Reason);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        }
    }
}