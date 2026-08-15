using Microsoft.EntityFrameworkCore;
using Telemetry.UserManagement.API.Features.Shared.Utils;
using Telemetry.UserManagement.Infrastructure.Database;
using Telemetry.UserManagement.Infrastructure.Database.Entities;

namespace Telemetry.UserManagement.API.Middlewares;

public class EnsureUserExistsMiddleware
{
    private readonly RequestDelegate _next;

    public EnsureUserExistsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = AuthUtils.GetUserIdFromClaimsPrincipal(context.User);

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                var exists = await dbContext.Users.AnyAsync(u => u.Id == userId, context.RequestAborted);

                if (!exists)
                {
                    dbContext.Users.Add(new User
                    {
                        Id = userId,
                        CreatedAtUtc = DateTime.UtcNow
                    });

                    await dbContext.SaveChangesAsync(context.RequestAborted);
                }
            }
        }

        await _next(context);
    }
}
