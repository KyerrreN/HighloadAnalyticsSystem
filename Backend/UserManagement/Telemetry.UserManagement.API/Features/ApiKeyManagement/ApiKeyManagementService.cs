using Microsoft.EntityFrameworkCore;
using Telemetry.UserManagement.Infrastructure.Logging;
using Telemetry.UserManagement.API.Features.ApiKeyManagement.CreateApiKey;
using Telemetry.UserManagement.Infrastructure.Database;
using Telemetry.UserManagement.Infrastructure.Database.Entities;
using Telemetry.Contracts.Interfaces;
using Telemetry.Contracts.Result;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement;

public sealed class ApiKeyManagementService : IApiKeyManagementService
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ApiKeyManagementService> _logger;
    private readonly IApiKeyGenerator _apiKeyGenerator;
    private readonly IApiKeyHasher _apiKeyHasher;

    public ApiKeyManagementService(
        AppDbContext dbContext,
        TimeProvider timeProvider,
        ILogger<ApiKeyManagementService> logger,
        IApiKeyGenerator apiKeyGenerator,
        IApiKeyHasher apiKeyHasher)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _logger = logger;
        _apiKeyGenerator = apiKeyGenerator;
        _apiKeyHasher = apiKeyHasher;
    }

    public async Task<Result<CreateApiKeyResponse>> CreateApiKeyAsync(Guid ownerId, Guid projectId, CreateApiKeyRequest request, CancellationToken ct = default)
    {
        var projectExists = await _dbContext.Projects
            .AnyAsync(p => p.Id == projectId && p.OwnerId == ownerId && !p.IsDeleted, ct);

        var rawKey = _apiKeyGenerator.GenerateRawKey();
        var keyHash = _apiKeyHasher.HashKey(rawKey);
        var prefix = _apiKeyGenerator.CreateDisplayPrefix(rawKey);

        var apiKey = new ApiKey
        {
            ProjectId = projectId,
            Name = request.Name.Trim(),
            KeyHash = keyHash,
            Prefix = prefix,
            IsRevoked = false,
            CreatedAtUtc = _timeProvider.GetUtcNow().UtcDateTime,
            ExpiresAtUtc = request.ExpiresAtUtc
        };

        try
        {
            _dbContext.ApiKeys.Add(apiKey);
            await _dbContext.SaveChangesAsync(ct);
            
            var response = new CreateApiKeyResponse(
                apiKey.Id,
                apiKey.ProjectId,
                apiKey.Name,
                rawKey,
                apiKey.Prefix,
                apiKey.CreatedAtUtc,
                apiKey.ExpiresAtUtc);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogApiKeyCreationFailed(ex, request.Name, projectId);
            return Result.Failed<CreateApiKeyResponse>(ApiKeyErrors.CreationFailed);
        }
    }

    public async Task<Result<IReadOnlyList<ApiKeyDto>>> GetApiKeysForProjectAsync(Guid ownerId, Guid projectId, CancellationToken ct = default)
    {
        try
        {
            var isProjectExist = await _dbContext.Projects
                .AsNoTracking()
                .AnyAsync(p => p.OwnerId == ownerId && p.Id == projectId && !p.IsDeleted, ct);

            if (!isProjectExist)
            {
                return Result.Failed<IReadOnlyList<ApiKeyDto>>(ApiKeyErrors.ProjectNotFound);
            }

            var keys = await _dbContext.ApiKeys
                .AsNoTracking()
                .Where(a => a.ProjectId == projectId && !a.IsRevoked)
                .OrderByDescending(a => a.CreatedAtUtc)
                .Select(a => new ApiKeyDto(
                    a.Id,
                    a.ProjectId,
                    a.Name,
                    a.Prefix,
                    a.IsRevoked,
                    a.CreatedAtUtc,
                    a.ExpiresAtUtc,
                    a.LastUsedAtUtc))
                .ToListAsync(ct);

            return keys;
        }
        catch (Exception ex)
        {
            _logger.LogApiKeyFetchFailed(ex, projectId);
            return Result.Failed<IReadOnlyList<ApiKeyDto>>(ApiKeyErrors.FetchFailed);
        }
    }

    public async Task<Result> RevokeApiKeyAsync(Guid projectId, Guid keyId, Guid ownerId, CancellationToken ct = default)
    {
        try
        {
            var apiKey = await _dbContext.ApiKeys
                .Include(k => k.Project)
                .FirstOrDefaultAsync(k => 
                    k.Id == keyId
                    && k.ProjectId == projectId
                    && k.Project.OwnerId == ownerId
                    && !k.Project.IsDeleted, ct);

            if (apiKey is null)
            {
                return Result.Failed(ApiKeyErrors.NotFound);
            }

            if (!apiKey.IsRevoked)
            {
                apiKey.IsRevoked = true;
                apiKey.RevokedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
                await _dbContext.SaveChangesAsync(ct);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogApiKeyRevokeFailed(ex, keyId, projectId);
            return Result.Failed(ApiKeyErrors.RevokeFailed);
        }
    }

    public async Task<Result<Guid>> ValidateApiKeyHashAsync(string keyHash, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyHash))
        {
            return Result.Failed<Guid>(ApiKeyErrors.InvalidHash);
        }

        try
        {
            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

            var apiKeyInfo = await _dbContext.ApiKeys
                .AsNoTracking()
                .Where(k => k.KeyHash == keyHash
                         && !k.IsRevoked
                         && !k.Project.IsDeleted
                         && (k.ExpiresAtUtc == null || k.ExpiresAtUtc > utcNow))
                .Select(k => new { k.ProjectId })
                .FirstOrDefaultAsync(ct);

            if (apiKeyInfo is null)
            {
                return Result.Failed<Guid>(ApiKeyErrors.InvalidOrExpired);
            }

            return apiKeyInfo.ProjectId;
        }
        catch (Exception ex)
        {
            _logger.LogApiKeyValidationError(ex);
            return Result.Failed<Guid>(ApiKeyErrors.ValidationFailed);
        }
    }
}

public interface IApiKeyManagementService
{
    Task<Result<CreateApiKeyResponse>> CreateApiKeyAsync(
        Guid ownerId,
        Guid projectId,
        CreateApiKeyRequest request,
        CancellationToken ct = default);

    Task<Result<IReadOnlyList<ApiKeyDto>>> GetApiKeysForProjectAsync(
        Guid ownerId,
        Guid projectId,
        CancellationToken ct = default);

    Task<Result> RevokeApiKeyAsync(
        Guid projectId,
        Guid keyId,
        Guid ownerId,
        CancellationToken ct = default);

    Task<Result<Guid>> ValidateApiKeyHashAsync(string keyHash, CancellationToken ct = default);
}
