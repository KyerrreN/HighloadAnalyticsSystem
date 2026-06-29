using System.Diagnostics;
using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.Observability.HighPerformanceLogging;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class TelemetryPublishWorker(
    ITelemetryEventChannel channel,
    IEventMessageBus messageBus,
    ILogger<TelemetryPublishWorker> logger) 
    : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new("Telemetry.Ingress.Tracing"); // todo: move name to constants

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogStarted();

        try
        {
            await foreach (var envelope in channel.ReadAllAsync(stoppingToken))
            {
                using var activity = ActivitySource.StartActivity(
                    "Kafka Publish Event",
                    ActivityKind.Producer,
                    envelope.TraceContext);

                activity?.SetTag("messaging.system", "kafka");
                activity?.SetTag("telemetry.event_name", envelope.Payload.EventName);

                await messageBus.PublishAsync(envelope.Payload, envelope.TraceContext, stoppingToken);
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
