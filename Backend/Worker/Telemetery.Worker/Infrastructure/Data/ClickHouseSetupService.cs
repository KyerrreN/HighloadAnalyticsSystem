using ClickHouse.Client.ADO;
using System.Data.Common;

namespace Telemetry.Worker.Infrastructure.Data;

public class ClickHouseSetupService : IHostedService
{
    private readonly string _connectionString;
    private readonly ILogger<ClickHouseSetupService> _logger;

    public ClickHouseSetupService(IConfiguration configuration, ILogger<ClickHouseSetupService> logger)
    {
        _connectionString = configuration.GetConnectionString("ClickHouse")
            ?? throw new ArgumentNullException(nameof(_connectionString), "ClickHouse connection string is missing"); // todo: resolve, perhaps options

        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // todo: high-performance logging
        _logger.LogInformation("Checking and initializing ClickHouse...");

        try
        {
            using DbConnection connection = new ClickHouseConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            using DbCommand command = connection.CreateCommand();

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS telemetry_events
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

            // todo: high-performance logging
            _logger.LogInformation("ClickHouse's scheme is initialized succesfully");
        }
        catch (Exception ex)
        {
            // todo: high-performance logging
            _logger.LogCritical(ex, "FATAL: Couldn't initialize ClickHouse schema. Check DB Connection or ConnectionString");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
