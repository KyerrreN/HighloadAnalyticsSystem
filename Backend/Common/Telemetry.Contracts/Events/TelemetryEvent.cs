using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Telemetry.Contracts.Events;

public record TelemetryEvent(
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
    public bool IsValid()
    {
        if (Properties.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        return true;
    }
}

// neat trick to use source gens
// instead of reflection when deserializing/serializing
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(TelemetryEvent))]
[JsonSerializable(typeof(EnvelopedEvent))]
public partial class IngressJsonContext : JsonSerializerContext { }