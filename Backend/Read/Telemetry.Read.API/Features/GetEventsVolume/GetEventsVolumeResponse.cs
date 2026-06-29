namespace Telemetry.Read.API.Features.GetEventsVolume;

public record EventVolumePoint(DateTime Timestamp, long TotalEvents);

public record GetEventsVolumeResponse(string EventName, EventGranularityEnum Granularity, List<EventVolumePoint> Items);
