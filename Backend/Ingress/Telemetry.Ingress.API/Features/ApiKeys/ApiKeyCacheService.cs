using Grpc.Core;
using Telemetry.Contracts.Grpc;
using Telemetry.Contracts.Interfaces;
using ZiggyCreatures.Caching.Fusion;

namespace Telemetry.Ingress.API.Features.ApiKeys;

public class ApiKeyCacheService : IApiKeyCacheService
{
    private const int ExpectedApiKeyLength = 67;
    private const string ApiKeyPrefix = "pk_";

    private readonly ILogger<ApiKeyCacheService> _logger;
    private readonly IFusionCache _cache;
    private readonly IApiKeyHasher _apiKeyHasher;
    private readonly ApiKeyValidation.ApiKeyValidationClient _grpcClient;

    public ApiKeyCacheService(
        ILogger<ApiKeyCacheService> logger,
        IFusionCache cache,
        IApiKeyHasher apiKeyHasher,
        ApiKeyValidation.ApiKeyValidationClient grpcClient)
    {
        _logger = logger;
        _cache = cache;
        _apiKeyHasher = apiKeyHasher;
        _grpcClient = grpcClient;
    }

    public async ValueTask<ApiKeyDetails?> ValidateApiKeyAsync(string rawApiKey, CancellationToken cancellationToken = default)
    {
        if (!IsValidFormat(rawApiKey)) return null;

        string keyHash = _apiKeyHasher.HashKey(rawApiKey);
        string cacheKey = $"apikey:{keyHash}";

        return await _cache.GetOrSetAsync<ApiKeyDetails?>(
            cacheKey,
            async (context, ct) =>
            {
                var remoteResult = await FetchFromUserManagementAsync(keyHash, ct);

                if (remoteResult is null)
                {
                    // negative caching, ddos protection
                    context.Options.Duration = TimeSpan.FromSeconds(10);
                    context.Options.DistributedCacheDuration = TimeSpan.FromSeconds(10);
                    return null;
                }

                return remoteResult;
            },
            token: cancellationToken);
    }

    private static bool IsValidFormat(string rawApiKey)
    {
        return !string.IsNullOrEmpty(rawApiKey)
            && rawApiKey.Length == ExpectedApiKeyLength
            && rawApiKey.StartsWith(ApiKeyPrefix, StringComparison.Ordinal);
    }

    private async Task<ApiKeyDetails?> FetchFromUserManagementAsync(string keyHash, CancellationToken ct)
    {
        try
        {
            var request = new ValidateApiKeyRequest
            {
                KeyHash = keyHash
            };

            var response = await _grpcClient.ValidateApiKeyAsync(request, cancellationToken: ct);

            if (!response.IsValid || string.IsNullOrEmpty(response.ProjectId))
            {
                return null;
            }

            return new ApiKeyDetails(response.ProjectId);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "gRPC call to UserManagement failed with status: {Status}", ex.Status); // todo: high-performance logging
            return null;
        }
    }
}

public interface IApiKeyCacheService
{
    ValueTask<ApiKeyDetails?> ValidateApiKeyAsync(string rawApiKey, CancellationToken cancellationToken = default);
}

public sealed record ApiKeyDetails(
    string ProjectId
);
