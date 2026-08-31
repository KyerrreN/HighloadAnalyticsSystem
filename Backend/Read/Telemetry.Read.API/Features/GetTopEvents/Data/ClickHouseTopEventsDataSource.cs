using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using Microsoft.Extensions.Options;
using Telemetry.Contracts.Constants;
using Telemetry.Read.Domain.Options;

namespace Telemetry.Read.API.Features.GetTopEvents.Data;

public class ClickHouseTopEventsDataSource : ITopEventsDataSource
{
    private readonly string _connectionString;

    public ClickHouseTopEventsDataSource(IOptions<ClickHouseOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<List<TopEventItem>> GetAsync(Guid projectId, DateTime from, DateTime to, int limit, CancellationToken ct)
    {
        using var connection = new ClickHouseConnection(_connectionString);
        await connection.OpenAsync(ct);

        using var command = connection.CreateCommand();
        command.CommandText = $"""
            SELECT 
                {TelemetryEventsTable.EventName}, 
                uniqExact({TelemetryEventsTable.EventId}) AS Count
            FROM {TelemetryEventsTable.TableName}
            WHERE {TelemetryEventsTable.ProjectId} = @projectId 
                AND {TelemetryEventsTable.Timestamp} >= @from 
                AND {TelemetryEventsTable.Timestamp} <= @to
            GROUP BY {TelemetryEventsTable.EventName}
            ORDER BY Count DESC
            LIMIT @limit
            """;

        command.AddParameter("projectId", projectId);
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
