using System.Security.Claims;
using Telemetry.UserManagement.API.Features.Shared;
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
                CurrentUser user,
                CancellationToken ct) =>
            {
                var response = await service.GetAllProjectsAsync(user.Id, ct);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response switch
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
