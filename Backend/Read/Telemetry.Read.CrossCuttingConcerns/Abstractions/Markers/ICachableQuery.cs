namespace Telemetry.Read.CrossCuttingConcerns.Abstractions.Markers;

public interface ICachableQuery
{
    string CacheKey { get;  }
    TimeSpan TimeToLive { get; }
}
