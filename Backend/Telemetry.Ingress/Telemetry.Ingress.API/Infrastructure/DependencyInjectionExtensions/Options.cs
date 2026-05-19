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
        } 
    }
}
