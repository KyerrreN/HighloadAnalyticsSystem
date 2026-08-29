using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;

public static class CacheExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection ConfigureCaching(IConfiguration configuration)
        {
            string redisConnectionString = configuration.GetConnectionString("Redis")
                ?? "localhost:6379"; // todo: options

            services.AddMemoryCache(opt =>
            {
                opt.SizeLimit = 1_000_000; // todo: options
                opt.CompactionPercentage = 0.2; // todo: options
            });

            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = redisConnectionString;
                opt.InstanceName = "telemetry:";
            });

            services.AddFusionCache()
                .WithDefaultEntryOptions(opt =>
                {
                    opt.Duration = TimeSpan.FromMinutes(2); // L1, todo: options
                    opt.DistributedCacheDuration = TimeSpan.FromDays(1); // L2, todo: options
                    opt.Size = 1;
                    opt.IsFailSafeEnabled = true;
                    opt.FailSafeThrottleDuration = TimeSpan.FromSeconds(30);
                })
                .WithSerializer(new FusionCacheSystemTextJsonSerializer())
                .WithRegisteredDistributedCache()
                .WithBackplane(new RedisBackplane(new RedisBackplaneOptions
                {
                    Configuration = redisConnectionString
                }));

            return services;
        }
    }
}
