using Telemetry.UserManagement.API.Options;

namespace Telemetry.UserManagement.API.Extensions;

public static class RegisterOptionsPattern
{
    extension (IServiceCollection services)
    {
        public IServiceCollection RegisterOptions(IConfiguration configuration)
        {
            services.Configure<KeycloakOptions>(configuration.GetSection(KeycloakOptions.SectionName));

            return services;
        }
    }
}
