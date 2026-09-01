namespace Telemetry.Read.API.Features.GetTopEvents;

public sealed record TopEventItem(string EventName, long Count);

public sealed record GetTopEventsResponse(List<TopEventItem> Items);
