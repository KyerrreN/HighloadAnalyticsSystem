using System.Security.Claims;
using Telemetry.UserManagement.API.Features.Shared;
using Telemetry.UserManagement.API.Features.Shared.Utils;

namespace Telemetry.UserManagement.API.Features.ProjectManagement.CreateProject;

public static class CreateProjectEndpoint
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapCreateProjectEndpoint()
        {
            endpoints.MapPost("/", async (
                CreateProjectRequestDto request,
                CurrentUser user,
                IProjectManagementService service,
                CancellationToken ct) =>
            {
                // todo: validation

                var response = await service.CreateProjectAsync(user.Id, request, ct);

                if (response.IsSuccess)
                {
                    return Results.Created($"/api/projects/{response.Value!.Id}", response.Value);
                }

                return response.Error.Code switch
                {
                    "Project.EmptyName" or "Project.AlreadyExists" =>
                        Results.BadRequest(new { error = response.Error.Message, code = response.Error.Code }),

                    _ => Results.Problem(
                        statusCode: StatusCodes.Status500InternalServerError,
                        detail: response.Error.Message)
                };
            })
                .WithName("CreateProject")
                .WithSummary("Create project for user")
                .WithDescription("Creates a project for current authenticated user");

            return endpoints;
        }
    }
}
