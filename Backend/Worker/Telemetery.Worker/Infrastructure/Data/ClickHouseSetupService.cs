using ClickHouse.Client.ADO;
using Microsoft.Extensions.Options;
using System.Data.Common;
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
                    ProjectApiKey String,
                    Timestamp DateTime64(3, 'UTC'),
                    EventId UUID,
                    EventName String,
                    ActorId String,
                    SessionId String,
                    Properties String
                )
                ENGINE = MergeTree()
                PARTITION BY toYYYYMMDD(Timestamp)
                ORDER BY (ProjectApiKey, EventName, Timestamp);
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
