using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using Microsoft.Extensions.Options;
using Telemetry.Read.CrossCuttingConcerns.Abstractions;
using Telemetry.Read.CrossCuttingConcerns.Options;

namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers;

public class GetDauMauHandler : IQueryHandler<GetDauMauQuery, List<DauMauResponse>>
{
    private readonly ClickHouseOptions _clickHouseOptions;

    public GetDauMauHandler(IOptions<ClickHouseOptions> clickHouseOptions)
    {
        _clickHouseOptions = clickHouseOptions.Value;
    }

    public async Task<List<DauMauResponse>> HandleAsync(GetDauMauQuery query, CancellationToken cancellationToken)
    {
        var result = new List<DauMauResponse>();

        using var connection = new ClickHouseConnection(_clickHouseOptions.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT 
                toStartOfDay(Timestamp) AS Date,
                uniqExact(ActorId) AS UniqueUsers
            FROM telemetry_events
            WHERE ProjectApiKey = @apiKey 
              AND Timestamp >= @from 
              AND Timestamp <= @to
            GROUP BY Date
            ORDER BY Date ASC
            """;

        command.AddParameter("apiKey", query.ProjectApiKey);
        command.AddParameter("from", query.From);
        command.AddParameter("to", query.To);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var date = reader.GetDateTime(0);

            var uniqueUsers = Convert.ToInt64(reader.GetValue(1));

            result.Add(new DauMauResponse(date, uniqueUsers));
        }

        return result;
    }
}
