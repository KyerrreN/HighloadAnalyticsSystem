using System.ComponentModel.DataAnnotations;

namespace Telemetry.Shared.Caching.Options;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    [Range(1_000, 10_000_000)]
    public required int MemoryCacheSizeLimit { get; set; }

    [Range(0.01, 0.90)]
    public required double CompactionPercentage { get; set; }

    public required TimeSpan L1Duration { get; set; }
    public required TimeSpan L2Duration { get; set; }
    public required TimeSpan FailSafeThrottleDuration { get; set; }
}
