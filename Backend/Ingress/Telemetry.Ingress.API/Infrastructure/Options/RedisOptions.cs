using System.ComponentModel.DataAnnotations;

namespace Telemetry.Ingress.API.Infrastructure.Options;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";

    [Required(AllowEmptyStrings = false, ErrorMessage = "Redis ConnectionString is required.")]
    public string ConnectionString { get; set; } = string.Empty;

    public string InstanceName { get; set; } = "IngressCache:";
}
