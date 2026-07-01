using System.Threading.Channels;
using Telemetry.Contracts.Interfaces;
using Telemetry.Contracts.Events;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class TelemetryEventChannel : ITelemetryEventChannel
{
    private readonly Channel<EnvelopedEvent> _channel;
    // todo: determine the right size for backpressure. seems too low for a moment
    // also calculate message size
    // maybe move it to config?
    private const int _channelCapacity = 100_000;
    
    public int Count => _channel.Reader.Count;

    public TelemetryEventChannel()
    {
        var options = new BoundedChannelOptions(_channelCapacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        };

        _channel = Channel.CreateBounded<EnvelopedEvent>(options);
    }

    public IAsyncEnumerable<EnvelopedEvent> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }

    public bool TryWrite(EnvelopedEvent @event)
    {
        return _channel.Writer.TryWrite(@event);
    }
}
