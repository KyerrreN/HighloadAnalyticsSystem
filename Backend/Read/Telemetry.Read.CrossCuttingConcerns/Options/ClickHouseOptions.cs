using System.ComponentModel.DataAnnotations;

namespace Telemetry.Read.CrossCuttingConcerns.Options;

public class ClickHouseOptions
{
    public const string SectionName = "ClickHouse";

    [Required(AllowEmptyStrings = false, ErrorMessage = $"\"{nameof(ConnectionString)}\" config is not present in configuration section \"{SectionName}\"")]
    public required string ConnectionString { get; set; }
}
