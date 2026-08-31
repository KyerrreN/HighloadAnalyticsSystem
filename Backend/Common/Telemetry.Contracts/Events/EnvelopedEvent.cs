namespace Telemetry.Contracts.Events;

public sealed record EnvelopedEvent(
    Guid ProjectId,
    TelemetryEvent Payload,
    string? TraceParent,
    DateTime ReceivedAt
);
