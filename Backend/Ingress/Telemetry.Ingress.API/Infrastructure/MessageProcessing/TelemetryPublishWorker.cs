using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.Observability.HighPerformanceLogging;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class TelemetryPublishWorker(
    ITelemetryEventChannel channel,
    IEventMessageBus messageBus,
    ILogger<TelemetryPublishWorker> logger) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogStarted();

        try
        {
            await foreach (var @event in channel.ReadAllAsync(stoppingToken))
            {
                await messageBus.PublishAsync(@event, stoppingToken);
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
