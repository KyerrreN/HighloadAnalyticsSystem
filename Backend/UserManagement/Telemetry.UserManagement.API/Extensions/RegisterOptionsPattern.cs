using Telemetry.UserManagement.API.Options;

namespace Telemetry.UserManagement.API.Extensions;

public static class RegisterOptionsPattern
{
    extension (IServiceCollection services)
    {
        public IServiceCollection RegisterOptions()
        {
            services.AddOptions<KeycloakOptions>()
                .BindConfiguration(KeycloakOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
