using Telemetry.Read.API.Features.Analytics.GetTopEvents;

namespace Telemetry.Read.API.Features.Analytics.GetTopEvents.Data;

public interface ITopEventsDataSource
{
    Task<List<TopEventItem>> GetAsync(
        Guid projectId,
        DateTime from,
        DateTime to,
        int limit,
        CancellationToken ct);
}
