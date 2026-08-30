namespace Telemetry.Ingress.API.Infrastructure.Options;

public sealed class RocksDbOptions
{
    public const string SectionName = "RocksDb";

    public string? ConnectionString { get; set; }
}
