using Telemetry.Read.Domain.Abstractions;
using Telemetry.Read.Domain.Abstractions.Markers;

namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers;

public record GetDauMauQuery(Guid ProjectId, DateTime From, DateTime? To) 
    : IQuery<List<DauMauResponse>>, ICachableQuery
{
    public string CacheKey => $"dau-mau:{ProjectId}:{From:yyyyMMdd}-{To?.ToString("yyyyMMdd") ?? "now"}";

    public TimeSpan TimeToLive => TimeSpan.FromMinutes(10);
}
