using Microsoft.AspNetCore.Authentication;

namespace Telemetry.Ingress.API.Infrastructure.Options;

public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";

    public string HeaderName { get; set; } = "X-API-Key";
}
