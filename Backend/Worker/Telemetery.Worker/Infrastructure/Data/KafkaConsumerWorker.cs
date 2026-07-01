using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Telemetry.Worker.Infrastructure.Options;
using Telemetry.Contracts.Events;
using Telemetry.Worker.Infrastructure.Data.Interfaces;
using Telemetry.Worker.Infrastructure.Observability.Logging;

namespace Telemetry.Worker.Infrastructure.Data;

public class KafkaConsumerWorker : BackgroundService
{
    private readonly KafkaOptions _kafkaOptions;
    private readonly BatchingOptions _batchingOptions;
    private readonly ITelemetrySink _sink;
    private readonly ILogger<KafkaConsumerWorker> _logger;

    public KafkaConsumerWorker(
        IOptions<KafkaOptions> options,
        ILogger<KafkaConsumerWorker> logger,
        IOptions<BatchingOptions> batchingOptions,
        ITelemetrySink sink)
    {
        _kafkaOptions = options.Value;
        _logger = logger;
        _batchingOptions = batchingOptions.Value;
        _sink = sink;
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

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig)
            .SetPartitionsAssignedHandler((c, partitions) =>
                _logger.LogPartitionsAssigned(string.Join(", ", partitions.Select(p => p.Partition.Value))))
            .Build();
        
        consumer.Subscribe(_kafkaOptions.TopicName);
        _logger.LogSubscribedToTopic(_kafkaOptions.TopicName);

        var batch = new List<TelemetryEvent>(_batchingOptions.MaxBatchSize);
        var lastFlushTime = DateTime.UtcNow;

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
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogDeserializationError(ex);
                    }
                }

                bool isBatchFull = batch.Count >= _batchingOptions.MaxBatchSize;
                bool isTimeUp = (DateTime.UtcNow - lastFlushTime) >= _batchingOptions.MaxWaitTime && batch.Count > 0;

                if (isBatchFull || isTimeUp)
                {
                    string reason = isBatchFull ? "Hit batch limit" : "Timeout";

                    _logger.LogFlushingBatch(batch.Count, reason);

                    try
                    {
                        await _sink.SaveBatchAsync(batch, stoppingToken);

                        consumer.Commit();

                        batch.Clear();
                        lastFlushTime = DateTime.UtcNow;

                        _logger.LogBatchSaved();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogSinkFlushError(ex);

                        await Task.Delay(2000, stoppingToken); // todo: potential error with batch overflow when kafka is down. investigate??
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogStopping();
        }
        finally
        {
            consumer.Close();
            _logger.LogDisconnected();
        }
    }
}
