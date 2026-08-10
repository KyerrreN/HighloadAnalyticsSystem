using System.Security.Claims;
using Telemetry.UserManagement.Infrastructure.Errors;

namespace Telemetry.UserManagement.API.Features.UserManagement.DeleteUser;

public static class DeleteUserEndpoint
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapDeleteUserEndpoint()
        {
            endpoints.MapDelete("/me", async (
                ClaimsPrincipal user,
                IUserManagementService userService,
                CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                                ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value; // todo: util class

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var result = await userService.DeleteUserAsync(userId, ct);

                if (result.IsSuccess)
                {
                    return Results.NoContent();
                }

                if (result.Error == UserErrors.NotFound)
                {
                    return Results.NotFound(new { error = result.Error.Message });
                }

                return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: result.Error.Message); // todo: for user, no need to return details of our error
            })
                .WithName("DeleteCurrentUser")
                .WithSummary("Delete current user account and all associated data");

            return endpoints;
        }
    }
}
