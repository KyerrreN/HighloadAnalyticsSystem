using System.Security.Claims;
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
                ClaimsPrincipal user,
                IProjectManagementService service,
                CancellationToken ct) =>
            {
                var userIdClaim = AuthUtils.GetUserIdFromClaimsPrincipal(user);

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var result = await service.GetProjectByIdAsync(userId, id, ct);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return result.Error.Code switch
                {
                    "ProjectErrors.NotFound" => Results.NotFound(new
                    {
                        error = result.Error.Message,
                        code = result.Error.Code
                    }),

                    _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: result.Error.Message)
                };
            })
                .WithName("GetProjectById")
                .WithSummary("Get project by its Id")
                .WithDescription("Retrieves details of a specific project by its Id");

            return endpoints;
        } 
    }
}
