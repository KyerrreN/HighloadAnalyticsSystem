using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using Microsoft.Extensions.Options;
using Telemetry.Read.CrossCuttingConcerns.Options;

namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers.Data;

public class ClickHouseDauMauDataSource : IDauMauDataSource
{
    private readonly string _connectionString;

    public ClickHouseDauMauDataSource(IOptions<ClickHouseOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<Dictionary<DateTime, long>> GetSparseDataAsync(string projectApiKey, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var sparseData = new Dictionary<DateTime, long>();

        using var connection = new ClickHouseConnection(_connectionString);
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

        command.AddParameter("apiKey", projectApiKey);
        command.AddParameter("from", from);
        command.AddParameter("to", to);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var date = reader.GetDateTime(0);
            var uniqueUsers = Convert.ToInt64(reader.GetValue(1));
            sparseData[date] = uniqueUsers;
        }

        return sparseData;
    }
}
