using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using Microsoft.Extensions.Options;
using Telemetry.Contracts.Constants;
using Telemetry.Read.Domain.Options;

namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers.Data;

public sealed class ClickHouseDauMauDataSource : IDauMauDataSource
{
    private readonly string _connectionString;

    public ClickHouseDauMauDataSource(IOptions<ClickHouseOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<Dictionary<DateTime, long>> GetSparseDataAsync(Guid projectId, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var sparseData = new Dictionary<DateTime, long>();

        using var connection = new ClickHouseConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.CommandText = $"""
            SELECT 
                toStartOfDay({TelemetryEventsTable.Timestamp}) AS Date,
                uniqExact({TelemetryEventsTable.ActorId}) AS UniqueUsers
            FROM {TelemetryEventsTable.TableName}
            WHERE {TelemetryEventsTable.ProjectId} = @projectId 
                AND {TelemetryEventsTable.Timestamp} >= @from 
                AND {TelemetryEventsTable.Timestamp} <= @to
            GROUP BY Date
            ORDER BY Date ASC
            """;

        command.AddParameter("projectId", projectId);
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
