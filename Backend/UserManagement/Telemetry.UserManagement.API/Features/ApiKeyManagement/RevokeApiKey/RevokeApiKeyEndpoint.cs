using System.Security.Claims;
using Telemetry.UserManagement.API.Features.Shared.Utils;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement.RevokeApiKey;

public static class RevokeApiKeyEndpoint
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapRevokeApiKeyEndpoint()
        {
            endpoints.MapDelete("/{projectId:guid}/keys/{keyId:guid}", async (
                Guid projectId,
                Guid keyId,
                ClaimsPrincipal user,
                IApiKeyManagementService apiKeyService,
                CancellationToken ct) =>
            {
                var userIdClaim = AuthUtils.GetUserIdFromClaimsPrincipal(user);

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var result = await apiKeyService.RevokeApiKeyAsync(projectId, keyId, userId, ct);

                if (result.IsSuccess)
                {
                    return Results.NoContent();
                }

                return result switch
                {
                    { Error: var err } when err == ApiKeyErrors.NotFound =>
                        Results.NotFound(new { error = err.Message, code = err.Code }),

                    _ => Results.Problem(
                        statusCode: StatusCodes.Status500InternalServerError,
                        detail: result.Error.Message)
                };
            })
                .WithName("RevokeApiKey")
                .WithSummary("Revoke API key")
                .WithDescription("Revokes an active API key. The key will immediately stop working.");

            return endpoints;
        }
    }
}
