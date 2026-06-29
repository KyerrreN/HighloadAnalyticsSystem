using Telemetry.Read.CrossCuttingConcerns.Options;

namespace Telemetry.Read.API.Infrastructure.Extensions;

public static class OptionsExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection RegisterOptions(IConfiguration configuration)
        {
            services.AddOptions<ClickHouseOptions>()
                .Bind(configuration.GetSection(ClickHouseOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
