using System.Security.Claims;
using Telemetry.UserManagement.API.Features.Shared;
using Telemetry.UserManagement.API.Features.Shared.Utils;

namespace Telemetry.UserManagement.API.Features.ProjectManagement.DeleteProject;

public static class DeleteProjectEndpoint
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapDeleteProjectEndpoint()
        {
            endpoints.MapDelete("/{id:guid}", async (
                Guid id,
                CurrentUser user,
                IProjectManagementService service,
                CancellationToken ct) =>
            {
                var result = await service.DeleteProjectAsync(user.Id, id, ct);

                if (result.IsSuccess)
                {
                    return Results.NoContent();
                }

                return result switch
                {
                    { Error: var err } when err == ProjectErrors.NotFound =>
                        Results.NotFound(new { error = err.Message, code = err.Code }),

                    _ => Results.Problem(
                        statusCode: StatusCodes.Status500InternalServerError,
                        detail: result.Error.Message)
                };
            })
                .WithName("DeleteProject")
                .WithSummary("Delete project by id")
                .WithDescription("Delete a project by id for a currently authenticated user");

            return endpoints;
        }
    }
}
