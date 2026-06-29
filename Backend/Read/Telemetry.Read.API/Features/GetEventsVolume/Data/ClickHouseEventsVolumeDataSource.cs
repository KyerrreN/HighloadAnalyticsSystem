using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using Microsoft.Extensions.Options;
using Telemetry.Read.CrossCuttingConcerns.Options;

namespace Telemetry.Read.API.Features.GetEventsVolume.Data;

public class ClickHouseEventsVolumeDataSource : IEventsVolumeDataSource
{
    private readonly string _connectionString;

    public ClickHouseEventsVolumeDataSource(IOptions<ClickHouseOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<Dictionary<DateTime, long>> GetAsync(string projectApiKey, DateTime from, DateTime to, EventGranularityEnum granularity, string? eventName, CancellationToken ct)
    {
        using var connection = new ClickHouseConnection(_connectionString);
        await connection.OpenAsync(ct);

        using var command = connection.CreateCommand();
        int minutes = (int)granularity;

        command.CommandText = """
            SELECT 
                toStartOfInterval(Timestamp, toIntervalMinute(@minutes)) AS Bucket,
                count() AS TotalEvents
            FROM telemetry_events
            WHERE ProjectApiKey = @apiKey 
              AND Timestamp >= @from 
              AND Timestamp <= @to
              AND (@eventName = '' OR EventName = @eventName)
            GROUP BY Bucket
            ORDER BY Bucket ASC
            """;

        command.AddParameter("apiKey", projectApiKey);
        command.AddParameter("from", from);
        command.AddParameter("to", to);
        command.AddParameter("eventName", string.IsNullOrWhiteSpace(eventName) ? string.Empty : eventName);
        command.AddParameter("minutes", (int)granularity);

        using var reader = await command.ExecuteReaderAsync(ct);
        var sparseData = new Dictionary<DateTime, long>();

        while (await reader.ReadAsync(ct))
        {
            var bucketTime = reader.GetDateTime(0);
            var totalEvents = Convert.ToInt64(reader.GetValue(1));

            sparseData[bucketTime] = totalEvents;
        }

        return sparseData;
    }
}
