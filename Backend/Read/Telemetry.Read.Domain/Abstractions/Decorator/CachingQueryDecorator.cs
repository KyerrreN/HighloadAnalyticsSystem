using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics;
using System.Text.Json;
using Telemetry.Contracts.Constants;
using Telemetry.Read.Domain.Abstractions.Markers;
using Telemetry.Read.Domain.OpenTelemetry;

namespace Telemetry.Read.Domain.Abstractions.Decorator;

public class CachingQueryDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _inner;
    private readonly IDistributedCache _cache;
    private readonly ReadApiMetrics _metrics;

    public CachingQueryDecorator(
        IQueryHandler<TQuery, TResponse> inner, 
        IDistributedCache cache, 
        ReadApiMetrics metrics)
    {
        _inner = inner;
        _cache = cache;
        _metrics = metrics;
    }

    public async Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        // todo: handle cache stampede
        if (query is not ICachableQuery cachableQuery)
        {
            return await _inner.HandleAsync(query, cancellationToken);
        }

        var cachedString = await _cache.GetStringAsync(cachableQuery.CacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedString))
        {
            _metrics.RecordCacheHit();

            Activity.Current?.SetTag(OtelTagConstants.CacheStatus, "HIT");
            Activity.Current?.SetTag(OtelTagConstants.CacheKey, cachableQuery.CacheKey);

            return JsonSerializer.Deserialize<TResponse>(cachedString)!;
        }

        _metrics.RecordCacheMiss();
        Activity.Current?.SetTag(OtelTagConstants.CacheStatus, "MISS");

        var response = await _inner.HandleAsync(query, cancellationToken);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cachableQuery.TimeToLive
        };
        await _cache.SetStringAsync(cachableQuery.CacheKey, JsonSerializer.Serialize(response), options, cancellationToken);

        return response;
    }
}
