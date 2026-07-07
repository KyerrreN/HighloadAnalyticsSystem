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
        var links = events
            .Where(x => !string.IsNullOrEmpty(x.TraceParent))
            .Select(x => new ActivityLink(ActivityContext.Parse(x.TraceParent!, null)))
            .ToList();

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
            e.Payload.ProjectApiKey,
            e.Payload.Timestamp,
            e.Payload.EventId.ToString(),
            e.Payload.EventName,
            e.Payload.ActorId ?? "",
            e.Payload.SessionId ?? "",
            e.Payload.Properties.ValueKind != JsonValueKind.Undefined ? e.Payload.Properties.GetRawText() : "{}"
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
