using Telemetry.Read.Domain.Abstractions;
using Telemetry.Read.Domain.Abstractions.Markers;

namespace Telemetry.Read.API.Features.GetTopEvents;

public sealed record GetTopEventsQuery(Guid ProjectId, DatePeriodEnum Period = DatePeriodEnum.Last7Days, int Limit = 10)
    : IQuery<GetTopEventsResponse>, ICachableQuery
{
    public string CacheKey => $"top-events:{ProjectId}_period:{Period}_limit:{Limit}";

    public TimeSpan TimeToLive => TimeSpan.FromMinutes(10);
}
