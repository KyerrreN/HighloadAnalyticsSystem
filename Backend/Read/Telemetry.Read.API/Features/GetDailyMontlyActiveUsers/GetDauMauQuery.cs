using Telemetry.Read.CrossCuttingConcerns.Abstractions;
using Telemetry.Read.CrossCuttingConcerns.Abstractions.Markers;

namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers;

public record GetDauMauQuery(string ProjectApiKey, DateTime From, DateTime? To) : IQuery<List<DauMauResponse>>, ICachableQuery
{
    public string CacheKey => $"dau-mau:{ProjectApiKey}:{From:yyyyMMdd}-{To?.ToString("yyyyMMdd") ?? "now"}";

    public TimeSpan TimeToLive => TimeSpan.FromMinutes(10);
}
