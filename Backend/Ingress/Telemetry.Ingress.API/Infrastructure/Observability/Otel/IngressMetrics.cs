using System.Diagnostics;
using System.Diagnostics.Metrics;
using Telemetry.Contracts.Interfaces;

namespace Telemetry.Ingress.API.Infrastructure.Observability.Otel;

public class IngressMetrics : IDisposable
{
    public const string MeterName = "Telemetry.Ingress";
    
    private readonly Meter _meter;
    private readonly Counter<long> _eventsReceivedCounter;
    private readonly Counter<long> _kafkaErrorsCounter;
    private readonly Counter<long> _channelRejectedCounter;

    public IngressMetrics(ITelemetryEventChannel channel)
    {
        _meter = new Meter(MeterName);

        _eventsReceivedCounter = _meter.CreateCounter<long>(
            name: OtelConstants.EventsReceivedCounterName,
            description: "Count of successfully received events");

        _kafkaErrorsCounter = _meter.CreateCounter<long>(
            name: OtelConstants.KafkaErrorsCounterName,
            description: "Count of errors when delivering messages to Kafka");

        _channelRejectedCounter = _meter.CreateCounter<long>(
            name: OtelConstants.ChannelRejectedCounterName,
            description: "Count of events rejected due to channel overflow");

        _meter.CreateObservableGauge(
            name: OtelConstants.ChannelSizeGaugeName,
            observeValue: () => channel.Count,
            description: "Current number of items in the in-memory channel");
    }

    public void RecordKafkaError(string topicName, string errorType)
    {
        var tags = new TagList
        {
            { OtelTagConstants.MessagingSystem, "kafka" },
            { OtelTagConstants.MessagingDestinationName, topicName },
            { OtelTagConstants.ErrorType, errorType }
        };

        _kafkaErrorsCounter.Add(1, tags);
    }

    public void RecordEventsReceived()
    {
        _eventsReceivedCounter.Add(1);
    }

    public void RecordChannelRejected()
    {
        _channelRejectedCounter.Add(1);
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}
