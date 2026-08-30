using Telemetry.Ingress.API.Infrastructure.Options;

namespace Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;

public static class Options
{
    extension(IServiceCollection services)
    {
        public void RegisterOptions(IConfiguration configuration)
        {
            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection(KafkaOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<RedisOptions>()
                .Bind(configuration.GetSection(RedisOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<CacheOptions>()
                .Bind(configuration.GetSection(CacheOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        } 
    }
}
