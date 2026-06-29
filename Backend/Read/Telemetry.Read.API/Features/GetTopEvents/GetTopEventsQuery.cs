using Telemetry.Read.CrossCuttingConcerns.Abstractions;
using Telemetry.Read.CrossCuttingConcerns.Abstractions.Markers;

namespace Telemetry.Read.API.Features.GetTopEvents;

public record GetTopEventsQuery(string ProjectApiKey, DatePeriodEnum Period = DatePeriodEnum.Last7Days, int Limit = 10)
    : IQuery<GetTopEventsResponse>, ICachableQuery
{
    public string CacheKey => $"top-events:{ProjectApiKey}_period:{Period}_limit:{Limit}";

    public TimeSpan TimeToLive => TimeSpan.FromMinutes(10);
}
