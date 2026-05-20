using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Telemetry.Contracts.Events;
using Telemetry.Worker.Infrastructure.Data.Interfaces;
using Telemetry.Worker.Infrastructure.Options;

namespace Telemetry.Worker.Infrastructure.Data;

public class ClickHouseTelemetrySinc : ITelemetrySink
{
    private readonly ClickHouseOptions _options;

    public ClickHouseTelemetrySinc(IOptions<ClickHouseOptions> options)
    {
        _options = options.Value;
    }

    public async Task SaveBatchAsync(IReadOnlyCollection<TelemetryEvent> events, CancellationToken cancellationToken)
    {
        var rows = events.Select(e => new object[]
        {
            e.ProjectApiKey,
            e.Timestamp,
            e.EventId.ToString(),
            e.EventName,
            e.ActorId ?? "",
            e.SessionId ?? "",
            e.Properties.ValueKind != JsonValueKind.Undefined ? e.Properties.GetRawText() : "{}"
        }).ToList();

        using var connection = new ClickHouseConnection(_options.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        using var bulkCopy = new ClickHouseBulkCopy(connection)
        {
            DestinationTableName = _options.TableName,
            BatchSize = events.Count,
        };

        await bulkCopy.InitAsync();
        await bulkCopy.WriteToServerAsync(rows, cancellationToken);
    }
}
