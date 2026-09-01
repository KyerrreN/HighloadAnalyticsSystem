using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Telemetry.Contracts.Events;

public sealed record TelemetryEvent(
    Guid? EventId,
    DateTimeOffset? Timestamp,

    // fun fact - source gens since .net 10
    [Required, StringLength(256)]
    string EventName,

    [StringLength(256)]
    string? ActorId,

    [StringLength(256)]
    string? SessionId,

    JsonElement Properties
)
{
    /// <summary>
    /// Validates properties of <see cref="TelemetryEvent"/>
    /// </summary>
    /// <returns><see cref="bool"/> - validation result</returns>
    public bool IsValid()
    {
        if (Properties.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Ensures that nullable values that HAVE to be filled ARE filled
    /// </summary>
    /// <param name="timeProvider">Time provider</param>
    /// <returns>A current instance of <see cref="TelemetryEvent"/></returns>
    public TelemetryEvent EnsureDefaults(TimeProvider timeProvider)
    {
        if (EventId.HasValue && Timestamp.HasValue)
        {
            return this;
        }

        return this with
        {
            EventId = EventId ?? Ulid.NewUlid().ToGuid(),
            Timestamp = Timestamp ?? timeProvider.GetUtcNow()
        };
    }
}

// neat trick to use source gens
// instead of reflection when deserializing/serializing
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(TelemetryEvent))]
[JsonSerializable(typeof(EnvelopedEvent))]
public sealed partial class TelemetryEventJsonContext : JsonSerializerContext { }