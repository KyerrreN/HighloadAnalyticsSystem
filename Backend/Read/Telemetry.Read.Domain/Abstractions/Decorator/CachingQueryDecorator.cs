using System.Diagnostics;
using Telemetry.Contracts.Constants;
using Telemetry.Read.Domain.Abstractions.Markers;
using Telemetry.Read.Domain.OpenTelemetry;
using ZiggyCreatures.Caching.Fusion;

namespace Telemetry.Read.Domain.Abstractions.Decorator;

public sealed class CachingQueryDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _inner;
    private readonly IFusionCache _cache;
    private readonly ReadApiMetrics _metrics;

    public CachingQueryDecorator(
        IQueryHandler<TQuery, TResponse> inner,
        IFusionCache cache, 
        ReadApiMetrics metrics)
    {
        _inner = inner;
        _cache = cache;
        _metrics = metrics;
    }

    public async Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        if (query is not ICachableQuery cachableQuery)
        {
            return await _inner.HandleAsync(query, cancellationToken);
        }

        bool isCacheMiss = false;

        var response = await _cache.GetOrSetAsync<TResponse>(
            cachableQuery.CacheKey,
            async (ctx, ct) =>
            {
                isCacheMiss = true;
                _metrics.RecordCacheMiss();
                Activity.Current?.SetTag(OtelTagConstants.CacheStatus, "MISS");

                return await _inner.HandleAsync(query, ct);
            },
            options => options
                .SetDuration(cachableQuery.TimeToLive)
                .SetDistributedCacheDuration(cachableQuery.TimeToLive),
            token: cancellationToken);

        if (!isCacheMiss)
        {
            _metrics.RecordCacheHit();
            Activity.Current?.SetTag(OtelTagConstants.CacheStatus, "HIT");
        }

        Activity.Current?.SetTag(OtelTagConstants.CacheKey, cachableQuery.CacheKey);

        return response;
    }
}
