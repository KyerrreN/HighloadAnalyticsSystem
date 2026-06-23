namespace Telemetry.Read.API.Features.GetTopEvents;

public record TopEventItem(string EventName, long Count);

public record GetTopEventsResponse(List<TopEventItem> Items);
