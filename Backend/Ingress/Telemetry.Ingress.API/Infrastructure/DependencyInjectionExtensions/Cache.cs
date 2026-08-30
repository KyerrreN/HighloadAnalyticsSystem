using Telemetry.Ingress.API.Infrastructure.Options;
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
            var redisOptions = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>()
                            ?? new RedisOptions();
            var cacheOptions = configuration.GetSection(CacheOptions.SectionName).Get<CacheOptions>()
                ?? new CacheOptions();

            // L1
            services.AddMemoryCache(opt =>
            {
                opt.SizeLimit = cacheOptions.MemoryCacheSizeLimit;
                opt.CompactionPercentage = cacheOptions.CompactionPercentage;
            });

            // L2
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = redisOptions.ConnectionString;
                opt.InstanceName = redisOptions.InstanceName;
            });

            services.AddFusionCache()
                .WithDefaultEntryOptions(opt =>
                {
                    opt.Duration = cacheOptions.L1Duration;
                    opt.DistributedCacheDuration = cacheOptions.L2Duration;
                    opt.Size = 1;
                    opt.IsFailSafeEnabled = true;
                    opt.FailSafeThrottleDuration = cacheOptions.FailSafeThrottleDuration;
                })
                .WithSerializer(new FusionCacheSystemTextJsonSerializer())
                .WithRegisteredDistributedCache()
                .WithBackplane(new RedisBackplane(new RedisBackplaneOptions
                {
                    Configuration = redisOptions.ConnectionString
                }));

            return services;
        }
    }
}
