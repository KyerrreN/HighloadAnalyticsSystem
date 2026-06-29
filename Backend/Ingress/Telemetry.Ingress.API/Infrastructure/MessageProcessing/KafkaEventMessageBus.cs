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
            // highload optimizations
            Acks = Acks.Leader,
            LingerMs = 5,
            CompressionType = CompressionType.Lz4
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _logger = logger;
        _metrics = metrics;
    }

    public Task PublishAsync(TelemetryEvent @event, ActivityContext traceContext, CancellationToken cancellationToken)
    {
        // construct message key/value
        var key = !string.IsNullOrWhiteSpace(@event.SessionId) ? @event.SessionId :
                  !string.IsNullOrWhiteSpace(@event.ActorId) ? @event.ActorId :
                  @event.ProjectApiKey;

        var value = JsonSerializer.Serialize(@event);

        // construct headers
        var headers = new Headers();
        var propagationContext = new PropagationContext(traceContext, default);
        Propagators.DefaultTextMapPropagator.Inject(propagationContext, headers, SetHeaders);

        var message = new Message<string, string>
        {
            Key = key,
            Value = value,
            Headers = headers
        };

        _producer.Produce(_options.TopicName, message, deliveryHandler =>
        {
            if (deliveryHandler.Error.IsError)
            {
                _logger.LogDeliveryError(deliveryHandler.Error.Reason);

                _metrics.KafkaErrorsCounter.Add(1, new KeyValuePair<string, object?>("reason", deliveryHandler.Error.Reason));
            }
        });

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}
