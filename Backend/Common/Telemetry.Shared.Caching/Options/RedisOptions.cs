using System.ComponentModel.DataAnnotations;

namespace Telemetry.Shared.Caching.Options;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";

    [Required(AllowEmptyStrings = false, ErrorMessage = "Redis ConnectionString is required.")]
    public required string ConnectionString { get; set; }

    public required string InstanceName { get; set; }
}
