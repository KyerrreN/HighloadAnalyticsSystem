using RocksDbSharp;

namespace Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;

public static class RocksDbDefaults
{
    public static readonly WriteOptions AsyncWriteOptions = new WriteOptions().SetSync(false);
}
