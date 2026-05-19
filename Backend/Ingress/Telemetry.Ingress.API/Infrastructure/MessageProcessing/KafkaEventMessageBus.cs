using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.Logging;
using Telemetry.Ingress.API.Infrastructure.Observability;
using Telemetry.Ingress.API.Infrastructure.Options;
using Telemetry.Contracts.Events;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class KafkaEventMessageBus : IEventMessageBus, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic = "telemetry-events";

    private readonly ILogger<KafkaEventMessageBus> _logger;
    private readonly IngressMetrics _metrics;

    public KafkaEventMessageBus(
        IOptions<KafkaOptions> kafkaOptions, 
        ILogger<KafkaEventMessageBus> logger, 
        IngressMetrics metrics)
    {
        var options = kafkaOptions.Value;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = options.BootstrapServers,
            // highload optimizations
            Acks = Acks.Leader,
            LingerMs = 5,
            CompressionType = CompressionType.Lz4
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _logger = logger;
        _metrics = metrics;
    }

    public Task PublishAsync(TelemetryEvent @event, CancellationToken cancellationToken)
    {
        var key = @event.ProjectApiKey;

        var value = JsonSerializer.Serialize(@event);

        var message = new Message<string, string>
        {
            Key = key,
            Value = value
        };

        _producer.Produce(_topic, message, deliveryHandler =>
        {
            if (deliveryHandler.Error.IsError)
            {
                _logger.LogKafkaDeliveryError(deliveryHandler.Error.Reason);

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
