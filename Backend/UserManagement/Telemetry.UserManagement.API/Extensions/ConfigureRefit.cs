using Microsoft.Extensions.Options;
using Refit;
using Telemetry.UserManagement.API.Features.Shared.Keycloak;
using Telemetry.UserManagement.API.Options;

namespace Telemetry.UserManagement.API.Extensions;

public static class ConfigureRefit
{
    extension (IServiceCollection services)
    {
        public IServiceCollection RegisterRefit()
        {
            services.AddRefitGeneratedClient<IKeycloakApi>()
                .ConfigureHttpClient((sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<KeycloakOptions>>().Value;

                    client.BaseAddress = options.BaseAddress;
                });

            return services;
        }
    }
}
