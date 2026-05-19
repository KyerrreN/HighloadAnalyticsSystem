using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Telemetry.Ingress.API.Infrastructure.Logging;
using Telemetry.Ingress.API.Infrastructure.Options;
using Telemetry.Ingress.Domain.Events;
using Telemetry.Ingress.Domain.Interfaces;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class KafkaEventMessageBus : IEventMessageBus, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic = "telemetry-events";

    private readonly ILogger<KafkaEventMessageBus> _logger;

    public KafkaEventMessageBus(IOptions<KafkaOptions> kafkaOptions, ILogger<KafkaEventMessageBus> logger)
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
