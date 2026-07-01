namespace Telemetry.Ingress.API.Infrastructure.Observability.Otel;

public static class OtelConstants
{
    public const string ActivitySourceName = "Telemetry.Ingress.Tracing";

    public const string EventsReceivedCounterName = "telemetry.ingress.events.received";
    public const string KafkaErrorsCounterName = "telemetry.ingress.kafka.errors";
    public const string ChannelRejectedCounterName = "telemetry.ingress.channel.rejected";
    public const string ChannelSizeGaugeName = "telemetry.ingress.channel.size";
}
