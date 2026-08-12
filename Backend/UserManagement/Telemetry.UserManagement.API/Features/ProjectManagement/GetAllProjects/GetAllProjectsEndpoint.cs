using System.Security.Claims;
using Telemetry.UserManagement.API.Features.Shared.Utils;

namespace Telemetry.UserManagement.API.Features.ProjectManagement.GetAllProjects;

public static class GetAllProjectsEndpoint
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapGetAllProjectsEndpoint()
        {
            endpoints.MapGet("/", async (
                IProjectManagementService service,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var userIdClaim = AuthUtils.GetUserIdFromClaimsPrincipal(user);

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var response = await service.GetAllProjectsAsync(userId, ct);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Error.Code switch
                {
                    _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: response.Error.Message)
                };
            })
                .WithName("GetProjects")
                .WithSummary("Get current user projects")
                .WithDescription("Retrieves a list of all projects owned by authenticated user");

            return endpoints;
        }
    }
}
