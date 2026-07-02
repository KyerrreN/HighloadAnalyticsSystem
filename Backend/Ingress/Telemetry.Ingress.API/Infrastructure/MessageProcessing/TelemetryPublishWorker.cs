using System.Diagnostics;
using System.Text.Json;
using RocksDbSharp;
using Telemetry.Contracts.Events;
using Telemetry.Contracts.Interfaces;
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
    private static readonly WriteOptions AsyncWriteOptions = new WriteOptions().SetSync(false);

    private const int BatchSize = 500;
    public const string PublishActivityName = "Kafka Publish Event";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogStarted();

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var keysToDelete = new List<byte[]>(BatchSize);
                var publishTasks = new List<Task>(BatchSize);

                using var writeBatch = new WriteBatch();
                bool hasPoisonPills = false;

                using (var iterator = db.NewIterator())
                {
                    iterator.SeekToFirst();

                    if (!iterator.Valid())
                    {
                        await Task.Delay(50, stoppingToken);
                        continue;
                    }

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
                            // todo: high performance logging
                            logger.LogError(ex, "Poison pill detected in WAL. Message corrupted and will be dropped.");
                            // todo: metric
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
                }

                if (publishTasks.Count == 0 && hasPoisonPills)
                {
                    db.Write(writeBatch, AsyncWriteOptions);
                    continue;
                }

                try
                {
                    await Task.WhenAll(publishTasks);

                    foreach (var key in keysToDelete)
                    {
                        writeBatch.Delete(key);
                    }

                    db.Write(writeBatch, AsyncWriteOptions);
                }
                catch (Exception ex)
                {
                    logger.LogProcessingError(nameof(TelemetryPublishWorker), ex);
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogCancelled();
        }
        catch (Exception ex)
        {
            logger.LogProcessingError(nameof(TelemetryPublishWorker), ex);
        }
    }

    private async Task PublishWithTracingAsync(EnvelopedEvent envelope, CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity(
            PublishActivityName,
            ActivityKind.Producer,
            envelope.TraceContext);

        activity?.SetTag(OtelTagConstants.MessagingSystem, "kafka");

        await messageBus.PublishAsync(envelope.Payload, envelope.TraceContext, stoppingToken);
    }
}