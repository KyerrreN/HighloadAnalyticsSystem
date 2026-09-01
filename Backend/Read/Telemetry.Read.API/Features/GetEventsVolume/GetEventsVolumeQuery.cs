using Telemetry.Read.Domain.Abstractions;
using Telemetry.Read.Domain.Abstractions.Markers;

namespace Telemetry.Read.API.Features.GetEventsVolume;

public sealed record GetEventsVolumeQuery(
    Guid ProjectId,
    DateOnly From,
    DateOnly To,
    EventGranularityEnum Granularity,
    string? EventName = null) : IQuery<GetEventsVolumeResponse>, ICachableQuery
{
    public string CacheKey => $"events-volume:{ProjectId}_from:{From:yyyyMMdd}_to:{To:yyyyMMdd}_gran:{Granularity}_event:{EventName ?? "all"}";

    public TimeSpan TimeToLive => TimeSpan.FromMinutes(5);
}
