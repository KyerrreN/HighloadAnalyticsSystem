using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Telemetry.Read.CrossCuttingConcerns.Abstractions.Markers;

namespace Telemetry.Read.CrossCuttingConcerns.Abstractions.Decorator;

public class CachingQueryDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _inner;
    private readonly IDistributedCache _cache;

    public CachingQueryDecorator(IQueryHandler<TQuery, TResponse> inner, IDistributedCache cache)
    {
        _inner = inner;
        _cache = cache;
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
            return JsonSerializer.Deserialize<TResponse>(cachedString)!;
        }

        var response = await _inner.HandleAsync(query, cancellationToken);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cachableQuery.TimeToLive
        };
        await _cache.SetStringAsync(cachableQuery.CacheKey, JsonSerializer.Serialize(response), options, cancellationToken);

        return response;
    }
}
