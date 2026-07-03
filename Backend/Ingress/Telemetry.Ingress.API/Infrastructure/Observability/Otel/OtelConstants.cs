namespace Telemetry.Ingress.API.Infrastructure.Observability.Otel;

public static class OtelConstants
{
    public const string ActivitySourceName = "Telemetry.Ingress.Tracing";

    public const string EventsReceivedCounterName = "telemetry.ingress.events.received";
    public const string KafkaErrorsCounterName = "telemetry.ingress.kafka.errors";
    public const string ChannelSizeGaugeName = "telemetry.ingress.channel.size";
    public const string PoisonPillsCounterName = "telemetry.ingress.events.poison_pills";
    public const string KafkaRejectedMessageCounter = "telemetry.ingress.events.permanent_rejections";
}
