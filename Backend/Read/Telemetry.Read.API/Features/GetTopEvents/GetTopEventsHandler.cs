using Telemetry.Read.API.Features.GetTopEvents.Data;
using Telemetry.Read.CrossCuttingConcerns.Abstractions;

namespace Telemetry.Read.API.Features.GetTopEvents;

public class GetTopEventsHandler(ITopEventsDataSource dataSource, TimeProvider timeProvider) : IQueryHandler<GetTopEventsQuery, GetTopEventsResponse>
{
    public async Task<GetTopEventsResponse> HandleAsync(GetTopEventsQuery query, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        (DateOnly from, DateOnly to) = query.Period switch
        {
            DatePeriodEnum.Today => (today, today),
            DatePeriodEnum.Yesterday => (today.AddDays(-1), today.AddDays(-1)),
            DatePeriodEnum.Last7Days => (today.AddDays(-7), today),
            DatePeriodEnum.Last30Days => (today.AddDays(-30), today),
            DatePeriodEnum.LastYear => (today.AddDays(-365), today),

            _ => throw new ArgumentOutOfRangeException(nameof(query.Period), "Provided period is not supported")
        };

        DateTime dbFrom = from.ToDateTime(TimeOnly.MinValue);
        DateTime dbTo = to.ToDateTime(TimeOnly.MaxValue);

        var data = await dataSource.GetAsync(query.ProjectApiKey, dbFrom, dbTo, query.Limit, cancellationToken);

        return new GetTopEventsResponse(data);
    }
}
