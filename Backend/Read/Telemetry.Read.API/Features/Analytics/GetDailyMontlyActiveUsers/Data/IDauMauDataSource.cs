namespace Telemetry.Read.API.Features.Analytics.GetDailyMontlyActiveUsers.Data;

public interface IDauMauDataSource
{
    Task<Dictionary<DateTime, long>> GetSparseDataAsync(
        Guid projectId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken);
}
