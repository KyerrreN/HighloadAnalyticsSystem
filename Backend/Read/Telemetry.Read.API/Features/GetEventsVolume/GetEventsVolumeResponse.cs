namespace Telemetry.Read.API.Features.GetEventsVolume;

public sealed record EventVolumePoint(DateTime Timestamp, long TotalEvents);

public sealed record GetEventsVolumeResponse(string EventName, EventGranularityEnum Granularity, List<EventVolumePoint> Items);
