using System.ComponentModel.DataAnnotations;

namespace Telemetry.Worker.Infrastructure.Options;

public class BatchingOptions
{
    public const string SectionName = "Batching";

    [Range(1, 100_000, ErrorMessage = "Batch size must be between 1 and 100,000")]
    public int MaxBatchSize { get; set; } = 10000;

    public TimeSpan MaxWaitTime { get; set; } = TimeSpan.FromSeconds(5);
}