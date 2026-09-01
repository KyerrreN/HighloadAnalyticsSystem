using Telemetry.UserManagement.API.Features.Shared.Utils;

namespace Telemetry.UserManagement.API.Features.Shared;

public sealed record CurrentUser(Guid Id)
{
    public static ValueTask<CurrentUser?> BindAsync(HttpContext context)
    {
        var userIdClaim = AuthUtils.GetUserIdFromClaimsPrincipal(context.User);

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return ValueTask.FromResult<CurrentUser?>(new CurrentUser(userId));
        }

        throw new BadHttpRequestException("Invalid or missing user ID claim.", StatusCodes.Status401Unauthorized);
    }
}
