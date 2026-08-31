using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using Telemetry.Contracts.Constants;
using Telemetry.Contracts.Events;
using Telemetry.Worker.Infrastructure.Data.Interfaces;
using Telemetry.Worker.Infrastructure.Observability.Otel;
using Telemetry.Worker.Infrastructure.Options;

namespace Telemetry.Worker.Infrastructure.Data;

public class ClickHouseTelemetrySinc : ITelemetrySink
{
    private readonly ClickHouseOptions _options;

    private static readonly ActivitySource _activitySource = new(OtelConstants.TelemetrySinkActivitySourceName);

    public ClickHouseTelemetrySinc(IOptions<ClickHouseOptions> options)
    {
        _options = options.Value;
    }

    public async Task SaveBatchAsync(IReadOnlyCollection<EnvelopedEvent> events, CancellationToken cancellationToken)
    {
        var links = new List<ActivityLink>();
        foreach (var x in events)
        {
            if (!string.IsNullOrEmpty(x.TraceParent) && ActivityContext.TryParse(x.TraceParent, null, out var context))
            {
                links.Add(new ActivityLink(context));
            }
        }

        using var activity = _activitySource.StartActivity(
            "ClickHouse Bulk Insert",
            kind: ActivityKind.Client,
            parentContext: default,
            tags: null,
            links: links);

        activity?.SetTag(OtelTagConstants.DatabaseName, "clickhouse");
        activity?.SetTag(OtelTagConstants.CollectionName, _options.TableName);
        activity?.SetTag(OtelTagConstants.DbBatchSize, events.Count);
        activity?.SetTag(OtelTagConstants.DbOperationName, "INSERT");

        var rows = events.Select(e => new object[]
        {
            e.ProjectId,
            e.Payload.EventId ?? Guid.NewGuid(),
            e.Payload.EventName,
            (e.Payload.Timestamp ?? e.ReceivedAt).UtcDateTime,
            e.ReceivedAt,
            (object?)e.Payload.ActorId ?? DBNull.Value,
            (object?)e.Payload.SessionId ?? DBNull.Value,
            e.Payload.Properties.ValueKind is not JsonValueKind.Undefined and not JsonValueKind.Null
                ? e.Payload.Properties.GetRawText()
                : "{}",
            (object?)e.TraceParent ?? DBNull.Value
        }).ToList();

        using var connection = new ClickHouseConnection(_options.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        using var bulkCopy = new ClickHouseBulkCopy(connection)
        {
            DestinationTableName = _options.TableName,
            BatchSize = events.Count,
            ColumnNames = TelemetryEventsTable.ColumnNames,
        };

        await bulkCopy.InitAsync();
        await bulkCopy.WriteToServerAsync(rows, cancellationToken);
    }
}
