using Telemetry.Contracts.Events;

namespace Telemetry.Contracts.Interfaces;

/// <summary>
/// Interface for writing to a Channel, which acts as a message buffer
/// </summary>
public interface ITelemetryEventChannel
{
    /// <summary>
    /// Try writing to a channel
    /// </summary>
    /// <param name="event"></param>
    /// <returns>true on success, false otherwise</returns>
    bool TryWrite(EnvelopedEvent @event);

    /// <summary>
    /// Read all messages from a channel
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="IAsyncEnumerable{T}"/> to asynchronously iterate through a collection<br/>Waits for the next element to pop in a channel</returns>
    IAsyncEnumerable<EnvelopedEvent> ReadAllAsync(CancellationToken cancellationToken);
}
