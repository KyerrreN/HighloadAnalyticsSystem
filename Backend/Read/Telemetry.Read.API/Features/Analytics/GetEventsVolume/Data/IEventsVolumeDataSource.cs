using Telemetry.Read.API.Features.Analytics.GetEventsVolume;

namespace Telemetry.Read.API.Features.Analytics.GetEventsVolume.Data;

public interface IEventsVolumeDataSource
{
    Task<Dictionary<DateTime, long>> GetAsync(
        Guid projectId,
        DateTime from,
        DateTime to,
        EventGranularityEnum granularity,
        string? eventName,
        CancellationToken ct);
}
