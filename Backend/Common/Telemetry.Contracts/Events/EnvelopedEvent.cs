namespace Telemetry.Contracts.Events;

public record EnvelopedEvent(
    TelemetryEvent Payload,
    string? TraceParent
);
