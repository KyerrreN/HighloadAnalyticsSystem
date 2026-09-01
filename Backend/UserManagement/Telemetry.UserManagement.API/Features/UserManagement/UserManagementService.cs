using Telemetry.UserManagement.API.Features.Shared.Keycloak;
using Telemetry.UserManagement.Infrastructure.Database;
using Telemetry.UserManagement.Infrastructure.Errors;
using Telemetry.UserManagement.Infrastructure.Logging;
using Telemetry.Contracts.Result;

namespace Telemetry.UserManagement.API.Features.UserManagement;

public sealed class UserManagementService : IUserManagementService
{
    private readonly AppDbContext _dbContext;
    private readonly IKeycloakAdminService _keycloakAdminService;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(AppDbContext dbContext, IKeycloakAdminService keycloakAdminService, ILogger<UserManagementService> logger)
    {
        _dbContext = dbContext;
        _keycloakAdminService = keycloakAdminService;
        _logger = logger;
    }

    public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FindAsync([userId], cancellationToken: ct);

        if (user is null)
        {
            return Result.Failed(UserErrors.NotFound);
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

        try
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(ct);

            var keycloakResult = await _keycloakAdminService.DeleteUserAsync(user.Id, ct);

            if (keycloakResult.IsFailure)
            {
                await transaction.RollbackAsync(ct);
                return Result.Failed(keycloakResult.Error);
            }

            await transaction.CommitAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);

            _logger.LogUserDeletionFailed(ex, userId);

            return Result.Failed(UserErrors.DeletionFailed);
        }
    }
}

public interface IUserManagementService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns>
    ///     <see cref="Result"/> - operation result. 
    ///     <see cref="Error"/> of <see cref="Result"/> is generated automatically in case of an error
    /// </returns>
    Task<Result> DeleteUserAsync(Guid userId, CancellationToken ct = default);
}
