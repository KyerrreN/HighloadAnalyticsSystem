using Telemetry.UserManagement.API.Features.Shared;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement.GetApiKeysForProject;

public static class GetApiKeysForProjectEndpoint
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapGetApiKeysForProjectEndpoint()
        {
            endpoints.MapGet("/{projectId:guid}/keys", async (
                Guid projectId,
                CurrentUser user,
                IApiKeyManagementService service,
                CancellationToken ct) =>
            {
                var result = await service.GetApiKeysForProjectAsync(user.Id, projectId, ct);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return result switch
                {
                    { Error: var err } when err == ApiKeyErrors.ProjectNotFound =>
                        Results.NotFound(new { error = err.Message, code = err.Code }),

                    _ => Results.Problem(
                        statusCode: StatusCodes.Status500InternalServerError,
                        detail: result.Error.Message)
                };
            })
                .WithName("GetProjectApiKeys")
                .WithSummary("Get list of API keys for project")
                .WithDescription("Retrieves a list of metadata for all API keys associated with the specified project.");

            return endpoints;
        }
    }
}
