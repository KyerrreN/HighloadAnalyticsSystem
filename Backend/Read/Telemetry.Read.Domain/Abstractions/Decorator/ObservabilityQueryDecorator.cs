using System.Diagnostics;
using Telemetry.Read.Domain.Abstractions.Enrichers;
using Telemetry.Read.Domain.OpenTelemetry;

namespace Telemetry.Read.Domain.Abstractions.Decorator;

public class ObservabilityQueryDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private static readonly ActivitySource _activitySource = new(OtelConstants.ActivitySourceName);

    private readonly IQueryHandler<TQuery, TResponse> _inner;
    private readonly IEnumerable<IActivityEnricher<TQuery>> _enrichers;

    public ObservabilityQueryDecorator(IQueryHandler<TQuery, TResponse> inner, IEnumerable<IActivityEnricher<TQuery>> enrichers)
    {
        _inner = inner;
        _enrichers = enrichers;
    }

    public async Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity($"Handle {typeof(TQuery).Name}", ActivityKind.Internal);

        if (activity is not null)
        {
            foreach (var enricher in _enrichers)
            {
                enricher.Enrich(activity, query);
            }
        }

        try
        {
            return await _inner.HandleAsync(query, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }
    }
}
