using Telemetry.Read.API.Features.GetDailyMontlyActiveUsers.Data;
using Telemetry.Read.CrossCuttingConcerns.Abstractions;

namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers;

public class GetDauMauHandler : IQueryHandler<GetDauMauQuery, List<DauMauResponse>>
{
    private readonly TimeProvider _timeProvider;
    private readonly IDauMauDataSource _dataSource;

    public GetDauMauHandler(TimeProvider timeProvider, IDauMauDataSource dataSource)
    {
        _timeProvider = timeProvider;
        _dataSource = dataSource;
    }

    public async Task<List<DauMauResponse>> HandleAsync(GetDauMauQuery query, CancellationToken cancellationToken)
    {
        var actualTo = query.To.HasValue
            ? query.To.Value.Date.AddDays(1).AddTicks(-1)
            : _timeProvider.GetUtcNow().UtcDateTime;

        var sparseData = await _dataSource.GetSparseDataAsync(
            query.ProjectApiKey,
            query.From,
            actualTo,
            cancellationToken);

        var result = new List<DauMauResponse>();

        for (var currentDay = query.From.Date; currentDay <= actualTo.Date; currentDay = currentDay.AddDays(1))
        {
            var usersCount = sparseData.TryGetValue(currentDay, out var count) ? count : 0;
            result.Add(new DauMauResponse(currentDay, usersCount));
        }

        return result;
    }
}
