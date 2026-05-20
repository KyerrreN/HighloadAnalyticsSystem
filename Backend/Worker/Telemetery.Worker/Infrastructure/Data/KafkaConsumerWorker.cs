using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Telemetry.Worker.Infrastructure.Options;
using Telemetry.Contracts.Events;

namespace Telemetry.Worker.Infrastructure.Data;

public class KafkaConsumerWorker : BackgroundService
{
    private readonly KafkaOptions _kafkaOptions;
    private readonly ILogger<KafkaConsumerWorker> _logger;

    // todo: options
    private readonly IConfiguration _configuration;

    private const int batchSize = 10_000;

    public KafkaConsumerWorker(
        IOptions<KafkaOptions> options, 
        ILogger<KafkaConsumerWorker> logger,
        IConfiguration configuration)
    {
        _kafkaOptions = options.Value;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _kafkaOptions.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        var topic = _kafkaOptions.TopicName;

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig)
            //.SetErrorHandler((_, e) => _logger.LogError("KAFKA ERROR: {Reason}", e.Reason))
            //.SetLogHandler((_, log) => _logger.LogInformation("KAFKA INTERNAL [{Facility}]: {Message}", log.Facility, log.Message))
            .SetPartitionsAssignedHandler((c, partitions) =>
            // todo: high-performance loggings
                _logger.LogInformation("Available partitions: {partitions}", string.Join(", ", partitions.Select(p => p.Partition.Value))))
            .Build();
        consumer.Subscribe(topic);

        // todo: high performance logging
        _logger.LogInformation("Succesfully subscribed to topic {topic}. Awaiting messages...", topic);

        // todo: does it make sense to move it to configurations?
        var batch = new List<TelemetryEvent>(batchSize);
        var maxWaitTime = TimeSpan.FromSeconds(5);
        var lastFlushTime = DateTime.UtcNow;

        ConsumeResult<Ignore, string>? lastConsumeResult = null;

        // necessary to not block the thread
        await Task.Yield();

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(100));

                if (consumeResult is not null && !string.IsNullOrWhiteSpace(consumeResult.Message.Value))
                {
                    try
                    {
                        var telemeteryEvent = JsonSerializer.Deserialize<TelemetryEvent>(consumeResult.Message.Value);

                        if (telemeteryEvent is not null)
                        {
                            batch.Add(telemeteryEvent);

                            lastConsumeResult = consumeResult;
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError("FATAL KAFKA CONSUME EXCEPTION: {Reason}. Offset: {Offset}", ex.Error.Reason, ex.ConsumerRecord?.Offset);
                    }
                    catch (JsonException ex)
                    {
                        // todo: high-performance logging
                        _logger.LogError(ex, "Error deserializing message from Kafka.");
                    }
                }

                bool isBatchFull = batch.Count >= batchSize;
                bool isTimeUp = (DateTime.UtcNow - lastFlushTime) >= maxWaitTime && batch.Count > 0;

                if (isBatchFull || isTimeUp)
                {
                    string reason = isBatchFull ? "Hit batch limit" : "Timeout";

                    // todo: high-performance logging
                    _logger.LogInformation("Flushing batch of {count} messages. Reason: {reason}", batch.Count, reason);

                    // todo: bulk insert
                    // todo: high-performance logging
                    try
                    {
                        var rows = batch.Select(e => new object[]
                        {
                            e.ProjectApiKey,
                            e.Timestamp,
                            e.EventId.ToString(),
                            e.EventName,
                            e.ActorId ?? "",
                            e.SessionId ?? "",
                            e.Properties.ValueKind != JsonValueKind.Undefined ? e.Properties.GetRawText() : "{}"
                        }).ToList();

                        var connectionString = _configuration.GetConnectionString("ClickHouse");
                        using var connection = new ClickHouseConnection(connectionString);
                        await connection.OpenAsync(stoppingToken);

                        using var bulkCopy = new ClickHouseBulkCopy(connection)
                        {
                            // todo: options
                            DestinationTableName = "telemetry_events",
                            BatchSize = batch.Count,
                        };

                        await bulkCopy.InitAsync();
                        await bulkCopy.WriteToServerAsync(rows, stoppingToken);

                        if (lastConsumeResult is not null)
                        {
                            consumer.Commit(lastConsumeResult);
                            lastConsumeResult = null;
                        }

                        batch.Clear();
                        lastFlushTime = DateTime.UtcNow;

                        // todo: high-performance logging
                        _logger.LogInformation("Batch succesfully saved to ClickHouse and commited to Kafka");
                    }
                    catch (Exception ex)
                    {
                        // todo: high-performance logging
                        _logger.LogError(ex, "Failed to flush batch to ClickHouse. Retrying...");

                        await Task.Delay(2000, stoppingToken);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // todo: high-performance logging
            _logger.LogInformation("Caught stopping signal. Stopping...");
        }
        finally
        {
            consumer.Close();
            // todo: high-performance logging
            _logger.LogInformation("Disconnected from Kafka");
        }
    }
}
