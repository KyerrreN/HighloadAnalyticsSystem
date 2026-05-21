using Telemetry.Read.CrossCuttingConcerns.Abstractions;
using Telemetry.Read.CrossCuttingConcerns.Abstractions.Markers;

namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers;

public record GetDauMauQuery(string ProjectApiKey, DateTime From, DateTime To) : IQuery<List<DauMauResponse>>, ICachableQuery
{
    // todo: constants maybe? or not worth it?
    public string CacheKey => $"dau-mau:{ProjectApiKey}:{DateTime.UtcNow:yyyy-MM-dd}";

    public TimeSpan TimeToLive => TimeSpan.FromMinutes(10);
}
