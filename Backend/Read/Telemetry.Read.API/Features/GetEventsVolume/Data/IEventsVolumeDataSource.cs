namespace Telemetry.Read.API.Features.GetEventsVolume.Data;

public interface IEventsVolumeDataSource
{
    Task<Dictionary<DateTime, long>> GetAsync(
        string projectApiKey,
        DateTime from,
        DateTime to,
        EventGranularityEnum granularity,
        string? eventName,
        CancellationToken ct);
}
