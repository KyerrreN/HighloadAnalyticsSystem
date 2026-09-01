using System.Diagnostics;
using System.Diagnostics.Metrics;
using Telemetry.Contracts.Constants;

namespace Telemetry.Ingress.API.Infrastructure.Observability.Otel;

public sealed class IngressMetrics : IDisposable
{
    public const string MeterName = "Telemetry.Ingress";
    
    private readonly Meter _meter;
    private readonly Counter<long> _eventsReceivedCounter;
    private readonly Counter<long> _kafkaErrorsCounter;
    private readonly Counter<long> _poisonPillsCounter;
    private readonly Counter<long> _permanentRejectionCounter;

    public IngressMetrics()
    {
        _meter = new Meter(MeterName);

        _eventsReceivedCounter = _meter.CreateCounter<long>(
            name: OtelConstants.EventsReceivedCounterName,
            description: "Count of successfully received events");

        _kafkaErrorsCounter = _meter.CreateCounter<long>(
            name: OtelConstants.KafkaErrorsCounterName,
            description: "Count of errors when delivering messages to Kafka");

        _poisonPillsCounter = _meter.CreateCounter<long>(
            name: OtelConstants.PoisonPillsCounterName,
            description: "Count of corrupted messages (poison pills) dropped from WAL");

        _permanentRejectionCounter = _meter.CreateCounter<long>(
            name: OtelConstants.KafkaRejectedMessageCounter,
            description: "Count of valid messages permanently rejected by Kafka and dropped");
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

    public void RecordPoisolPill(string exceptionType)
    {
        var tags = new TagList
        {
            { OtelTagConstants.ErrorType, exceptionType }
        };

        _poisonPillsCounter.Add(1, tags);
    }

    public void RecordPermanentRejection(string reason)
    {
        var tags = new TagList
        {
            { OtelTagConstants.MessagingSystem, "kafka" },
            { OtelTagConstants.ErrorType, reason }
        };

        _permanentRejectionCounter.Add(1, tags);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _meter.Dispose();
    }
}
