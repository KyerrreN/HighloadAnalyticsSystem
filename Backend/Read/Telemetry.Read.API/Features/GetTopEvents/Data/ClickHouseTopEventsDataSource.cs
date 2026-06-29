using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using Microsoft.Extensions.Options;
using Telemetry.Read.CrossCuttingConcerns.Options;

namespace Telemetry.Read.API.Features.GetTopEvents.Data;

public class ClickHouseTopEventsDataSource : ITopEventsDataSource
{
    private readonly string _connectionString;

    public ClickHouseTopEventsDataSource(IOptions<ClickHouseOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<List<TopEventItem>> GetAsync(string projectApiKey, DateTime from, DateTime to, int limit, CancellationToken ct)
    {
        using var connection = new ClickHouseConnection(_connectionString);
        await connection.OpenAsync(ct);

        using var command = connection.CreateCommand();
        command.CommandText = """
           SELECT EventName, count() AS Count
           FROM telemetry_events
           WHERE ProjectApiKey = @apiKey 
           AND Timestamp >= @from 
           AND Timestamp <= @to
           GROUP BY EventName
           ORDER BY Count DESC
           LIMIT @limit
           """;

        command.AddParameter("apiKey", projectApiKey);
        command.AddParameter("from", from);
        command.AddParameter("to", to);
        command.AddParameter("limit", limit);

        using var reader = await command.ExecuteReaderAsync(ct);

        var result = new List<TopEventItem>(limit);
        while (await reader.ReadAsync(ct))
        {
            var eventName = reader.GetString(0);

            var count = Convert.ToInt64(reader.GetValue(1));

            result.Add(new TopEventItem(eventName, count));
        }

        return result;
    }
}
