using System.Diagnostics.Metrics;

namespace Telemetry.Read.Domain.OpenTelemetry;

public sealed class ReadApiMetrics
{
    public const string MeterName = "Telemetry.Read.Metrics";

    private readonly Meter _meter;
    private readonly Counter<long> _cacheCounter;

    public ReadApiMetrics()
    {
        _meter = new Meter(MeterName);

        _cacheCounter = _meter.CreateCounter<long>(
            name: OtelConstants.RequestCacheCounterName,
            unit: "{requests}",
            description: "Total number of cache requests (hits and misses)");
    }

    public void RecordCacheHit()
    {
        _cacheCounter.Add(1, new KeyValuePair<string, object?>("status", "hit"));
    }

    public void RecordCacheMiss()
    {
        _cacheCounter.Add(1, new KeyValuePair<string, object?>("status", "miss"));
    }
}
