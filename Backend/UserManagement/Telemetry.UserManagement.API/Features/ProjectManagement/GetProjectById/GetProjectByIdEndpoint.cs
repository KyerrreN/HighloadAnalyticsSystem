using System.Security.Claims;
using Telemetry.UserManagement.API.Features.Shared;
using Telemetry.UserManagement.API.Features.Shared.Utils;

namespace Telemetry.UserManagement.API.Features.ProjectManagement.GetProjectById;

public static class GetProjectByIdEndpoint
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapGetProjectByIdEndpoint()
        {
            endpoints.MapGet("/{id:guid}", async (
                Guid id,
                CurrentUser user,
                IProjectManagementService service,
                CancellationToken ct) =>
            {
                var result = await service.GetProjectByIdAsync(user.Id, id, ct);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
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
                .WithName("GetProjectById")
                .WithSummary("Get project by its Id")
                .WithDescription("Retrieves details of a specific project by its Id");

            return endpoints;
        } 
    }
}
