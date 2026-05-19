using System.Text.Json;

namespace Telemetry.Ingress.Domain.Events;

public record TelemetryEvent(
    Guid EventId,
    string ProjectApiKey,
    DateTimeOffset Timestamp,
    string EventName,
    string? ActorId,
    string? SessionId,
    JsonElement Properties
);
