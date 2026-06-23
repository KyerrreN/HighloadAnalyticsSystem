namespace Telemetry.Read.API.Features.GetTopEvents.Data;

public interface ITopEventsDataSource
{
    Task<List<TopEventItem>> GetAsync(
        string projectApiKey,
        DateTime from,
        DateTime to,
        int limit,
        CancellationToken ct);
}
