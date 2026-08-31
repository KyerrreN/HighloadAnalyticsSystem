using System.ComponentModel.DataAnnotations;

namespace Telemetry.Ingress.API.Infrastructure.Options;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    [Range(1_000, 10_000_000)]
    public int MemoryCacheSizeLimit { get; set; } = 1_000_000;

    [Range(0.01, 0.90)]
    public double CompactionPercentage { get; set; } = 0.2;

    public TimeSpan L1Duration { get; set; } = TimeSpan.FromMinutes(2);
    public TimeSpan L2Duration { get; set; } = TimeSpan.FromDays(1);
    public TimeSpan FailSafeThrottleDuration { get; set; } = TimeSpan.FromSeconds(30);
}
