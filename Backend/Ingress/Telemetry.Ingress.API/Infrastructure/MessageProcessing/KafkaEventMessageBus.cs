using Confluent.Kafka;
using Microsoft.Extensions.Options;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Telemetry.Contracts.Events;
using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.Logging;
using Telemetry.Ingress.API.Infrastructure.Observability.Otel;
using Telemetry.Ingress.API.Infrastructure.Options;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class KafkaEventMessageBus : IEventMessageBus, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventMessageBus> _logger;
    private readonly IngressMetrics _metrics;
    private readonly KafkaOptions _options;
    private readonly Action<Headers, string, string> SetHeaders = (headers, key, value) =>
    {
        headers.Remove(key); // avoid duplicates
        headers.Add(key, Encoding.UTF8.GetBytes(value));
    };

    public KafkaEventMessageBus(
        IOptions<KafkaOptions> kafkaOptions,
        ILogger<KafkaEventMessageBus> logger,
        IngressMetrics metrics)
    {
        _options = kafkaOptions.Value;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageTimeoutMs = 30000, // 30s
            LingerMs = 5,
            CompressionType = CompressionType.Lz4,
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _logger = logger;
        _metrics = metrics;
    }

    /// <summary>
    /// Publishes message to kafka asynchronously
    /// </summary>
    /// <param name="event">Telemetry event to send</param>
    /// <param name="traceContext">Trace context for OTEL</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ProduceException{TKey, TValue}"></exception>
    /// <exception cref="Exception"></exception>
    public async Task PublishAsync(TelemetryEvent @event, ActivityContext traceContext, DateTime receivedAt, CancellationToken cancellationToken)
    {
        var key = !string.IsNullOrWhiteSpace(@event.SessionId) ? @event.SessionId :
                  !string.IsNullOrWhiteSpace(@event.ActorId) ? @event.ActorId :
                  @event.ProjectApiKey;

        var value = JsonSerializer.Serialize(@event);

        var headers = new Headers();
        var propagationContext = new PropagationContext(traceContext, default);
        Propagators.DefaultTextMapPropagator.Inject(propagationContext, headers, SetHeaders);

        SetHeaders(headers, "receivedat", receivedAt.ToString("O"));

        var message = new Message<string, string>
        {
            Key = key,
            Value = value,
            Headers = headers
        };

        try
        {
            var deliveryResult = await _producer.ProduceAsync(_options.TopicName, message, cancellationToken);

            if (deliveryResult.Status != PersistenceStatus.Persisted)
            {
                throw new Exception($"Message was not persisted. Status: {deliveryResult.Status}");
            }
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogDeliveryError(ex.Error.Reason);
            _metrics.RecordKafkaError(_options.TopicName, ex.Error.Reason);

            throw;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        // graceful shutdown
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}