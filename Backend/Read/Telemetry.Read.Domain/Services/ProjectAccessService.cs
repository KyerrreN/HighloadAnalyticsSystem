using Grpc.Core;
using Microsoft.Extensions.Logging;
using Telemetry.Contracts.Grpc;
using Telemetry.Contracts.Result;
using Telemetry.Read.Domain.Logging.Errors;
using ZiggyCreatures.Caching.Fusion;

namespace Telemetry.Read.Domain.Services;

public sealed class ProjectAccessService : IProjectAccessService
{
    private readonly IFusionCache _cache;
    private readonly ProjectAccess.ProjectAccessClient _grpcClient;
    private readonly ILogger<ProjectAccessService> _logger;

    private static readonly TimeSpan AccessCacheDuration = TimeSpan.FromMinutes(10);

    public ProjectAccessService(IFusionCache cache, ProjectAccess.ProjectAccessClient grpcClient, ILogger<ProjectAccessService> logger)
    {
        _cache = cache;
        _grpcClient = grpcClient;
        _logger = logger;
    }

    public async Task<bool> HasAccessAsync(Guid userId, Guid projectId, CancellationToken ct = default)
    {
        string cacheKey = "access:project:" + projectId.ToString() + ":user:" + userId.ToString();

        try
        {
            return await _cache.GetOrSetAsync<bool>(
                cacheKey,
                async (ctx, cancellationToken) =>
                {
                    var request = new CheckProjectAccessRequest
                    {
                        ProjectId = projectId.ToString(),
                        UserId = userId.ToString()
                    };

                    var response = await _grpcClient.CheckProjectAccessAsync(request, cancellationToken: cancellationToken);
                    return response.HasAccess;
                },
                options => options
                    .SetDuration(AccessCacheDuration)
                    .SetDistributedCacheDuration(AccessCacheDuration),
                token: ct);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Failed to receive response for project access via gRPC");

            return false;
        }
    }
}

public interface IProjectAccessService
{
    Task<bool> HasAccessAsync(Guid userId, Guid projectId, CancellationToken ct = default);
}
