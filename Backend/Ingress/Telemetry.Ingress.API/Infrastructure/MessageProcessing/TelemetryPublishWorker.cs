using Confluent.Kafka;
using System.Diagnostics;
using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.Observability.HighPerformanceLogging;
using Telemetry.Ingress.API.Infrastructure.Observability.Otel;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class TelemetryPublishWorker(
    ITelemetryEventChannel channel,
    IEventMessageBus messageBus,
    ILogger<TelemetryPublishWorker> logger) 
    : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new(OtelConstants.ActivitySourceName);

    public const string PublishActivityName = "Kafka Publish Event";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogStarted();

        try
        {
            await foreach (var envelope in channel.ReadAllAsync(stoppingToken))
            {
                bool isPublished = false;

                while (!isPublished && !stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using var activity = ActivitySource.StartActivity(
                            PublishActivityName,
                            ActivityKind.Producer,
                            envelope.TraceContext);

                        activity?.SetTag(OtelTagConstants.MessagingSystem, "kafka");
                        activity?.SetTag(OtelTagConstants.TelemetryEventName, envelope.Payload.EventName);

                        await messageBus.PublishAsync(envelope.Payload, envelope.TraceContext, stoppingToken);

                        isPublished = true;
                    }
                    catch (ProduceException<string, string> ex) when (ex.Error.Code == ErrorCode.Local_QueueFull)
                    {
                        await Task.Delay(100, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogProcessingError(nameof(TelemetryPublishWorker), ex);
                        break;
                    }
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
}
