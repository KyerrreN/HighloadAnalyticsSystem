namespace Telemetry.Contracts.Events;

public record EnvelopedEvent(
    Guid ProjectId,
    TelemetryEvent Payload,
    string? TraceParent,
    DateTime ReceivedAt
);
