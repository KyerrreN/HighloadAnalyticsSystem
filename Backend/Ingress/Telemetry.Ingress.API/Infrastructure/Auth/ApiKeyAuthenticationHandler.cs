using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Telemetry.Ingress.API.Infrastructure.Options;

namespace Telemetry.Ingress.API.Infrastructure.Auth;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base (options, logger, encoder) { }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var apiKeys))
        {
            return AuthenticateResult.NoResult();
        }

        var providedApiKey = apiKeys.FirstOrDefault();
        if (string.IsNullOrEmpty(providedApiKey))
        {
            return AuthenticateResult.NoResult();
        }

        bool isValid = providedApiKey.StartsWith("pk_"); // todo: could be improved, hardcoded
        string projectId = "test_project_id_123";

        if (!isValid)
        {
            return AuthenticateResult.Fail("Invalid API Key provided");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, projectId),
            new Claim("projectId", projectId) // todo: constants
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
