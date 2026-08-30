using System.ComponentModel.DataAnnotations;

namespace Telemetry.Ingress.API.Infrastructure.Options;

public sealed class GrpcOptions
{
    public const string SectionName = "GrpcServices";

    [Required]
    [Url]
    public string UserManagementUrl { get; set; } = string.Empty;
}
