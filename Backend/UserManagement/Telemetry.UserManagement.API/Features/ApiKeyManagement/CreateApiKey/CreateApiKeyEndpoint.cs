using System.Security.Claims;
using Telemetry.UserManagement.API.Features.Shared.Utils;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement.CreateApiKey;

public static class CreateApiKeyEndpoint
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapCreateApiKeyEndpoint()
        {
            endpoints.MapPost("/{projectId:guid}/keys", async (
                Guid projectId,
                ClaimsPrincipal user,
                CreateApiKeyRequest dto,
                IApiKeyManagementService service,
                CancellationToken ct) =>
            {
                // todo: validation
                var userIdClaim = AuthUtils.GetUserIdFromClaimsPrincipal(user);

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var response = await service.CreateApiKeyAsync(userId, projectId, dto, ct);

                if (response.IsSuccess)
                {
                    return Results.Created($"/api/projects/{projectId}/keys/{response.Value!.Id}", response.Value);
                }

                return response switch
                {
                    { Error: var err } when err == ApiKeyErrors.EmptyName =>
                    Results.BadRequest(new { error = err.Message, code = err.Code }),

                    { Error: var err } when err == ApiKeyErrors.ProjectNotFound =>
                        Results.NotFound(new { error = err.Message, code = err.Code }),

                    _ => Results.Problem(
                        statusCode: StatusCodes.Status500InternalServerError,
                        detail: response.Error.Message)
                };
            })
                .WithName("CreateApiKey")
                .WithSummary("Generate a new API key for project")
                .WithDescription("Generates a new API key. The raw key is returned ONLY once in the response!"); 
            
            return endpoints;
        }
    }
}
