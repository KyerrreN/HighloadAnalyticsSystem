using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Telemetry.Contracts.Utils;

public static class AuthUtils
{
    public static string? GetUserIdFromClaimsPrincipal(ClaimsPrincipal user)
    {
        return user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }
}
