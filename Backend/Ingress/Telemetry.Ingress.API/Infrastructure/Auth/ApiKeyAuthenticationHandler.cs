using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Telemetry.Ingress.API.Features.ApiKeys;
using Telemetry.Ingress.API.Infrastructure.Options;

namespace Telemetry.Ingress.API.Infrastructure.Auth;

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IApiKeyCacheService apiKeyCacheService)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
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

        var result = await apiKeyCacheService.ValidateApiKeyAsync(providedApiKey, Context.RequestAborted);
        if (result.IsFailure)
        {
            return AuthenticateResult.Fail(result.Error.Message);
        }

        var apiKeyDetails = result.Value!;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, apiKeyDetails.ProjectId),
            new Claim("projectId", apiKeyDetails.ProjectId) // todo: constants
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
