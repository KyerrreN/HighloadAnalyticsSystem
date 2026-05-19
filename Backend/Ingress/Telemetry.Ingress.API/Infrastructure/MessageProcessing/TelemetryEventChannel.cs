using System.Threading.Channels;
using Telemetry.Contracts.Interfaces;
using Telemetry.Contracts.Events;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class TelemetryEventChannel : ITelemetryEventChannel
{
    private readonly Channel<TelemetryEvent> _channel;
    private const int _channelCapacity = 100_000; // todo: determine the right size. Perhaps limit API consumers property size

    public TelemetryEventChannel()
    {
        var options = new BoundedChannelOptions(_channelCapacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        };

        _channel = Channel.CreateBounded<TelemetryEvent>(options);
    }

    public IAsyncEnumerable<TelemetryEvent> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }

    public bool TryWrite(TelemetryEvent @event)
    {
        return _channel.Writer.TryWrite(@event);
    }
}
