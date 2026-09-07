using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telemetry.Shared.Caching.Options;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Telemetry.Shared.Caching;

public static class CachingExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection AddHybridCaching(IConfiguration configuration)
        {
            var redisOptions = configuration
                .GetRequiredSection(RedisOptions.SectionName)
                .Get<RedisOptions>()!;

            var cacheOptions = configuration
                .GetRequiredSection(CacheOptions.SectionName)
                .Get<CacheOptions>()!;

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
