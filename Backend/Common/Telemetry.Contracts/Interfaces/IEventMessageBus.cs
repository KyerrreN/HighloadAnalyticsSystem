using Telemetry.Contracts.Events;

namespace Telemetry.Contracts.Interfaces;

/// <summary>
/// An interface to publish messages
/// </summary>
public interface IEventMessageBus
{
    Task PublishAsync(TelemetryEvent @event, CancellationToken cancellationToken);
}
