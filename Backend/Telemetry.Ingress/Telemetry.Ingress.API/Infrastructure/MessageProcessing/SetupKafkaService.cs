using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;
using Telemetry.Ingress.API.Infrastructure.Logging;
using Telemetry.Ingress.API.Infrastructure.Options;

namespace Telemetry.Ingress.API.Infrastructure.MessageProcessing;

public class SetupKafkaService : IHostedService
{
    private readonly KafkaOptions _options;
    private readonly ILogger<SetupKafkaService> _logger;

    public SetupKafkaService(IOptions<KafkaOptions> options, ILogger<SetupKafkaService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _options.BootstrapServers,
        }).Build();

        var topicName = _options.TopicName;
        var partitionCount = _options.PartitionCount;
        var replicationFactor = _options.ReplicationFactor;

        try
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));

            if (!metadata.Topics.Exists(t => t.Topic == topicName))
            {
                _logger.LogKafkaTopicNotExist();

                await adminClient.CreateTopicsAsync(
                [
                    new TopicSpecification
                    {
                        Name = topicName,
                        NumPartitions = partitionCount,
                        ReplicationFactor = (short)replicationFactor
                    }
                ]);

                _logger.LogKafkaTopicSuccesfullyCreated(topicName);
            }
        }
        catch (CreateTopicsException e) when (e.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
        {
            _logger.LogKafkaTopicAlreadyExist(topicName);
        }
        catch (Exception e)
        {
            _logger.LogKafkaTopicUnknownCreationError(topicName, e);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
