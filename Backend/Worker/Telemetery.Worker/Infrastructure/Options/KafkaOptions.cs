using System.ComponentModel.DataAnnotations;

namespace Telemetry.Worker.Infrastructure.Options;

public class KafkaOptions
{
    public const string SectionName = "Kafka";

    [Required(AllowEmptyStrings = false, ErrorMessage = $"\"{nameof(BootstrapServers)}\" config is not present in the configuration section \"{SectionName}\"")]
    public required string BootstrapServers { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = $"\"{nameof(TopicName)}\" config is not present in the configuration section \"{SectionName}\"")]
    public required string TopicName { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = $"\"{nameof(GroupId)}\" config is not present in the configuration section \"{SectionName}\"")]
    public required string GroupId { get; set; }
}
