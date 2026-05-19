using System.Diagnostics.Metrics;

namespace Telemetry.Ingress.API.Infrastructure.Observability;

public class IngressMetrics
{
    public const string MeterName = "Telemetry.Ingress.API";
    private readonly Meter _meter;

    public Counter<long> EventsReceivedCounter { get; set; }

    public Counter<long> KafkaErrorsCounter { get; set; }

    public IngressMetrics()
    {
        _meter = new Meter(MeterName);

        EventsReceivedCounter = _meter.CreateCounter<long>(
            name: "telemetery_events_received_total",
            description: "Count of success events");

        KafkaErrorsCounter = _meter.CreateCounter<long>(
            name: "telemetery_kafka_errors_total",
            description: "Count of errors to deliver messages to Kafka");
    }
}
