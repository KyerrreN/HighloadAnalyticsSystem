namespace Telemetry.Ingress.API.Infrastructure.Observability.Otel;

public static class OtelTagConstants
{
    // semantic conventions
    public const string MessagingSystem = "messaging.system";
    public const string MessagingDestinationName = "messaging.destination.name";
    public const string ErrorType = "error.type";

    // custom
    public const string TelemetryEventName = "telemetry.event_name";
}
