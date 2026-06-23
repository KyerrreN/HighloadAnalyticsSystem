using Telemetry.Read.CrossCuttingConcerns.Abstractions;
using Telemetry.Read.CrossCuttingConcerns.Abstractions.Markers;

namespace Telemetry.Read.API.Features.GetEventsVolume;

public record GetEventsVolumeQuery(
    string ProjectApiKey,
    DateOnly From,
    DateOnly To,
    EventGranularityEnum Granularity,
    string? EventName = null) : IQuery<GetEventsVolumeResponse>, ICachableQuery
{
    public string CacheKey => $"events-volume:{ProjectApiKey}_from:{From:yyyyMMdd}_to:{To:yyyyMMdd}_gran:{Granularity}_event:{EventName ?? "all"}";

    public TimeSpan TimeToLive => TimeSpan.FromMinutes(5);
}
