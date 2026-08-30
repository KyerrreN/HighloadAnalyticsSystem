using Telemetry.Contracts.Result;

namespace Telemetry.Ingress.API.Features.ApiKeys;

public static class ApiKeyErrors
{
    public static readonly Error InvalidFormat = new(
        "ApiKey.InvalidFormat",
        "Provided API key format is invalid."
    );

    public static readonly Error InvalidOrExpired = new(
        "ApiKey.InvalidOrExpired",
        "API key is invalid, revoked, expired, or project was deleted."
    );
}
