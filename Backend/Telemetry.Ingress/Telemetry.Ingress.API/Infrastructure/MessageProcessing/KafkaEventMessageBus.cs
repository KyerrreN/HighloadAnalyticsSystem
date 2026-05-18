using Confluent.Kafka;
using System.Text.Json;
using Telemetry.Ingress.Domain.Events;
using Telemetry.Ingress.Domain.Interfaces;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class KafkaEventMessageBus : IEventMessageBus, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic = "telemetry-events";

    private readonly ILogger<KafkaEventMessageBus> _logger;

    public KafkaEventMessageBus(IConfiguration configuration, ILogger<KafkaEventMessageBus> logger)
    {
        // todo: Options
        var bootstrapServers = configuration["Kafka:BootstrapServers"];

        // todo: all configs must be present. Else - don't let an app start up
        if (string.IsNullOrEmpty(bootstrapServers))
        {
            throw new ArgumentNullException(nameof(bootstrapServers), "Error: no config found for bootstrap servers");
        }

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
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
                // todo: high-performance logging
                _logger.LogError("Couldn't deliver message to Kafka. Reason: {reason}", deliveryHandler.Error.Reason);
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
