using Telemetry.Ingress.API.Infrastructure.Logging;
using Telemetry.Ingress.Domain.Interfaces;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class TelemetryPublishWorker(
    ITelemetryEventChannel channel,
    IEventMessageBus messageBus,
    ILogger<TelemetryPublishWorker> logger) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogBackgroundWorkerStarted();

        try
        {
            await foreach (var @event in channel.ReadAllAsync(stoppingToken))
            {
                await messageBus.PublishAsync(@event, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogBackgroundWorkerCancelled();
        }
        catch (Exception ex)
        {
            logger.LogProcessingError(nameof(TelemetryPublishWorker), ex);
        }
    }
}
