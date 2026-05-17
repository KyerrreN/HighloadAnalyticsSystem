using Telemetry.Ingress.Domain.Events;

namespace Telemetry.Ingress.Domain.Interfaces;

/// <summary>
/// An interface to publish messages
/// </summary>
public interface IEventMessageBus
{
    Task PublishAsync(TelemetryEvent @event, CancellationToken cancellationToken);
}
