using Telemetry.Ingress.Domain.Events;

namespace Telemetry.Ingress.Domain.Interfaces;

// todo: comments
public interface ITelemetryEventChannel
{
    bool TryWrite(TelemetryEvent @event);

    IAsyncEnumerable<TelemetryEvent> ReadAllAsync(CancellationToken cancellationToken);
}
