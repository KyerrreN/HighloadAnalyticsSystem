using System.ComponentModel.DataAnnotations;

namespace Telemetry.Ingress.API.Infrastructure.Options;

public class KafkaOptions
{
    public const string SectionName = "Kafka";

    [Required(AllowEmptyStrings = false, ErrorMessage = $"\"{nameof(BootstrapServers)}\" config is not present in the configuration section \"{SectionName}\"")]
    public required string BootstrapServers { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = $"\"{nameof(TopicName)}\" config is not present in configuration section \"{SectionName}\"")]
    public required string TopicName { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = $"\"{nameof(PartitionCount)}\" must be greater than 0 in section \"{SectionName}\"")]
    public required int PartitionCount { get; set; }

    [Range(1, short.MaxValue, ErrorMessage = $"\"{nameof(ReplicationFactor)}\" must be greater than 0 in section \"{SectionName}\"")]
    public required int ReplicationFactor { get; set; }
}
