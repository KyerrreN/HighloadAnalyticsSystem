using Microsoft.EntityFrameworkCore;
using Refit;
using Telemetry.UserManagement.API.Features.ApiKeyManagement.CreateApiKey;
using Telemetry.UserManagement.Infrastructure.Database;
using Telemetry.UserManagement.Infrastructure.Database.Entities;
using Telemetry.UserManagement.Infrastructure.Result;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement;

public class ApiKeyManagementService : IApiKeyManagementService
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ApiKeyManagementService> _logger;
    private readonly IApiKeyGenerator _apiKeyGenerator;

    public ApiKeyManagementService(
        AppDbContext dbContext, 
        TimeProvider timeProvider, 
        ILogger<ApiKeyManagementService> logger, 
        IApiKeyGenerator apiKeyGenerator)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _logger = logger;
        _apiKeyGenerator = apiKeyGenerator;
    }

    public async Task<Result<CreateApiKeyResponse>> CreateApiKeyAsync(Guid ownerId, Guid projectId, CreateApiKeyRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) // todo: move to validation
        {
            return Result.Failed<CreateApiKeyResponse>(ApiKeyErrors.EmptyName);
        }

        var projectExists = await _dbContext.Projects
            .AnyAsync(p => p.Id == projectId && p.OwnerId == ownerId && !p.IsDeleted, ct);

        var rawKey = _apiKeyGenerator.GenerateRawKey();
        var keyHash = _apiKeyGenerator.HashKey(rawKey);
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
            _logger.LogError(ex, "Failed to create API key {KeyName} for project {ProjectId}", request.Name, projectId); // high-performance logging
            return Result.Failed<CreateApiKeyResponse>(ApiKeyErrors.CreationFailed);
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
}
