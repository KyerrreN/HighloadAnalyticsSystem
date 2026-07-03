using Confluent.Kafka;
using RocksDbSharp;
using System.Diagnostics;
using System.Text.Json;
using Telemetry.Contracts.Events;
using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;
using Telemetry.Ingress.API.Infrastructure.Logging;
using Telemetry.Ingress.API.Infrastructure.Observability.HighPerformanceLogging;
using Telemetry.Ingress.API.Infrastructure.Observability.Otel;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class TelemetryPublishWorker(
    RocksDb db,
    IEventMessageBus messageBus,
    ILogger<TelemetryPublishWorker> logger)
    : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new(OtelConstants.ActivitySourceName);
    private const int BatchSize = 500;
    public const string PublishActivityName = "Kafka Publish Event";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogStarted();

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    bool hasProcessedData = await TryProcessNextBatchAsync(stoppingToken);

                    if (!hasProcessedData)
                    {
                        await Task.Delay(50, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogProcessingError(nameof(TelemetryPublishWorker), ex);
                    await Task.Delay(1000, stoppingToken); // todo: retry policy
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
        using var iterator = db.NewIterator();
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
            db.Write(writeBatch, RocksDbDefaults.AsyncWriteOptions);
            return true;
        }

        await Task.WhenAll(publishTasks);

        foreach (var key in keysToDelete)
        {
            writeBatch.Delete(key);
        }

        db.Write(writeBatch, RocksDbDefaults.AsyncWriteOptions);

        return true;
    }

    private async Task PublishWithTracingAsync(EnvelopedEvent envelope, CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity(
            PublishActivityName,
            ActivityKind.Producer,
            envelope.TraceContext);

        activity?.SetTag(OtelTagConstants.MessagingSystem, "kafka");

        try
        {
            await messageBus.PublishAsync(envelope.Payload, envelope.TraceContext, stoppingToken);
        }
        catch (ProduceException<string, string> ex)
        {
            if (ex.Error.IsFatal)
            {
                throw;
            }

            logger.LogKafkaMessageRejected(envelope.Payload.EventId, ex);
            // todo: metric
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        }
    }
}