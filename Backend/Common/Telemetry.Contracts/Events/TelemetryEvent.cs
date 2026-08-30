using System.Text.Json;

namespace Telemetry.Contracts.Events;

public record TelemetryEvent(
    Guid? EventId,
    DateTimeOffset? Timestamp,
    string EventName,
    string? ActorId,
    string? SessionId,
    JsonElement Properties
);
