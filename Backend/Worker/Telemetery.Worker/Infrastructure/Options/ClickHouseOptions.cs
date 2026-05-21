using System.ComponentModel.DataAnnotations;

namespace Telemetry.Worker.Infrastructure.Options;

public class ClickHouseOptions
{
    public const string SectionName = "ClickHouse";

    [Required(AllowEmptyStrings = false, ErrorMessage = "ClickHouse ConnectionString is required")]
    public required string ConnectionString { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string TableName { get; set; } = "telemetry_events";
}
