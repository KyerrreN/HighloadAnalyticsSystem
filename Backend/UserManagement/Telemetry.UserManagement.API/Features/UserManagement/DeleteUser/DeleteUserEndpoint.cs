using System.Security.Claims;
using Telemetry.UserManagement.API.Features.Shared;
using Telemetry.UserManagement.API.Features.Shared.Utils;
using Telemetry.UserManagement.Infrastructure.Errors;

namespace Telemetry.UserManagement.API.Features.UserManagement.DeleteUser;

public static class DeleteUserEndpoint
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapDeleteUserEndpoint()
        {
            endpoints.MapDelete("/me", async (
                CurrentUser user,
                IUserManagementService userService,
                CancellationToken ct) =>
            {
                var result = await userService.DeleteUserAsync(user.Id, ct);

                if (result.IsSuccess)
                {
                    return Results.NoContent();
                }

                return result switch
                {
                    { Error: var err } when err == UserErrors.NotFound =>
                        Results.NotFound(new { error = err.Message, code = err.Code }),

                    _ => Results.Problem(
                        statusCode: StatusCodes.Status500InternalServerError,
                        detail: result.Error.Message)
                };
            })
                .WithName("DeleteCurrentUser")
                .WithSummary("Delete current user account and all associated data");

            return endpoints;
        }
    }
}
