using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Telemetry.Worker.Infrastructure.Options;
using Telemetry.Contracts.Events;
using Telemetry.Worker.Infrastructure.Data.Interfaces;
using Telemetry.Worker.Infrastructure.Observability.Logging;
using Telemetry.Worker.Infrastructure.Observability.Otel;
using System.Text;

namespace Telemetry.Worker.Infrastructure.Data;

public class KafkaConsumerWorker : BackgroundService
{
    private readonly KafkaOptions _kafkaOptions;
    private readonly BatchingOptions _batchingOptions;
    private readonly ITelemetrySink _sink;
    private readonly ILogger<KafkaConsumerWorker> _logger;
    private readonly WorkerMetrics _metrics;
    private readonly TimeProvider _timeProvider;

    public KafkaConsumerWorker(
        IOptions<KafkaOptions> options,
        ILogger<KafkaConsumerWorker> logger,
        IOptions<BatchingOptions> batchingOptions,
        ITelemetrySink sink,
        WorkerMetrics metrics,
        TimeProvider timeProvider)
    {
        _kafkaOptions = options.Value;
        _logger = logger;
        _batchingOptions = batchingOptions.Value;
        _sink = sink;
        _metrics = metrics;
        _timeProvider = timeProvider;
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

        var batch = new List<EnvelopedEvent>(_batchingOptions.MaxBatchSize);
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
                            string? traceParent = null;
                            DateTime receivedAt = _timeProvider.GetUtcNow().UtcDateTime;

                            if (consumeResult.Message.Headers is not null)
                            {
                                if (consumeResult.Message.Headers.TryGetLastBytes("traceparent", out var headerBytes))
                                {
                                    traceParent = Encoding.UTF8.GetString(headerBytes);
                                }

                                if (consumeResult.Message.Headers.TryGetLastBytes("receivedat", out var receivedAtBytes))
                                {
                                    var receivedAtStr = Encoding.UTF8.GetString(receivedAtBytes);
                                    if (DateTime.TryParse(receivedAtStr, out var parsedDate))
                                    {
                                        receivedAt = parsedDate;
                                    }
                                }
                            }

                            var envelopedEvent = new EnvelopedEvent(telemeteryEvent, traceParent, receivedAt);

                            batch.Add(envelopedEvent);
                        }
                    }
                    catch (JsonException ex)
                    {
                        _metrics.RecordPoisonPill();
                        _logger.LogDeserializationError(ex);
                    }
                }

                bool isBatchFull = batch.Count >= _batchingOptions.MaxBatchSize;
                bool isTimeUp = (DateTime.UtcNow - lastFlushTime) >= _batchingOptions.MaxWaitTime && batch.Count > 0;

                if (isBatchFull || isTimeUp)
                {
                    string reason = isBatchFull ? "Hit batch limit" : "Timeout";
                    _logger.LogFlushingBatch(batch.Count, reason);

                    bool isBatchSaved = false;

                    while (!isBatchSaved && !stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            await _sink.SaveBatchAsync(batch, stoppingToken);

                            consumer.Commit();

                            _metrics.RecordEventsConsumed(batch.Count);
                            _metrics.RecordBatchSize(batch.Count);

                            batch.Clear();
                            lastFlushTime = DateTime.UtcNow;
                            isBatchSaved = true;

                            _logger.LogBatchSaved();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogSinkFlushError(ex);

                            await Task.Delay(2000, stoppingToken);
                        }
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
