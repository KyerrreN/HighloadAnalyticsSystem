using Telemetry.Contracts.Events;

namespace Telemetry.Worker.Infrastructure.Data.Interfaces;

/// <summary>
/// An interface to sink telemetery data into a persistent storage
/// </summary>
public interface ITelemetrySink
{
    /// <summary>
    /// Save a batch of telemetery events into a persitent storage
    /// </summary>
    /// <param name="events">Event batch</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SaveBatchAsync(IReadOnlyCollection<TelemetryEvent> events, CancellationToken cancellationToken);
}
