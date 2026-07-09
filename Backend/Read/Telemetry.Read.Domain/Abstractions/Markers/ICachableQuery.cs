namespace Telemetry.Read.Domain.Abstractions.Markers;

public interface ICachableQuery
{
    string CacheKey { get;  }
    TimeSpan TimeToLive { get; }
}
