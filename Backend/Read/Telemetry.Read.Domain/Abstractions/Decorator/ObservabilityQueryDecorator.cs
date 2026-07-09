using System.Diagnostics;
using Telemetry.Contracts.Constants;
using Telemetry.Read.Domain.Abstractions.Markers;
using Telemetry.Read.Domain.OpenTelemetry;
using Telemetry.Read.Domain.Utils;

namespace Telemetry.Read.Domain.Abstractions.Decorator;

public class ObservabilityQueryDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private static readonly ActivitySource _activitySource = new(OtelConstants.ActivitySourceName);
    private readonly IQueryHandler<TQuery, TResponse> _inner;

    public ObservabilityQueryDecorator(IQueryHandler<TQuery, TResponse> inner)
    {
        _inner = inner;
    }

    public async Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity($"Handle ${typeof(TQuery).Name}", ActivityKind.Internal);

        if (query is IProjectApiQuery projectApiQuery)
        {
            var hashed = HashUtils.HashApiKey(projectApiQuery.ProjectApiKey);
            activity?.SetTag(OtelTagConstants.ProjectApiKeyHash, hashed);
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
