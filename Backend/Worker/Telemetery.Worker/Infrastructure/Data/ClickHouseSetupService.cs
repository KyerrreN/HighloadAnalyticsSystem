using ClickHouse.Client.ADO;
using Microsoft.Extensions.Options;
using System.Data.Common;
using Telemetry.Contracts.Constants;
using Telemetry.Worker.Infrastructure.Observability.Logging;
using Telemetry.Worker.Infrastructure.Options;

namespace Telemetry.Worker.Infrastructure.Data;

public class ClickHouseSetupService : IHostedService
{
    private readonly ClickHouseOptions _options;
    private readonly ILogger<ClickHouseSetupService> _logger;

    public ClickHouseSetupService(
        IOptions<ClickHouseOptions> options, 
        ILogger<ClickHouseSetupService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInitializingClickHouse();

        try
        {
            using DbConnection connection = new ClickHouseConnection(_options.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            using DbCommand command = connection.CreateCommand();

            command.CommandText = $@"
                CREATE TABLE IF NOT EXISTS {_options.TableName}
                (
                    {TelemetryEventsTable.ProjectId} UUID,
                    {TelemetryEventsTable.EventId} UUID,
                    {TelemetryEventsTable.EventName} String,
                    {TelemetryEventsTable.Timestamp} DateTime64(3, 'UTC'),
                    {TelemetryEventsTable.ReceivedAt} DateTime64(3, 'UTC'),
                    {TelemetryEventsTable.ActorId} Nullable(String),
                    {TelemetryEventsTable.SessionId} Nullable(String),
                    {TelemetryEventsTable.Properties} String,
                    {TelemetryEventsTable.TraceParent} Nullable(String)
                )
                ENGINE = ReplacingMergeTree()
                PARTITION BY toMonday({TelemetryEventsTable.ReceivedAt})
                ORDER BY ({TelemetryEventsTable.ProjectId}, {TelemetryEventsTable.EventName}, {TelemetryEventsTable.Timestamp}, {TelemetryEventsTable.EventId});
            ";

            await command.ExecuteNonQueryAsync(cancellationToken);

            _logger.LogClickHouseInitialized();
        }
        catch (Exception ex)
        {
            _logger.LogClickHouseInitializationFailed(ex);
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
