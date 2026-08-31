using Microsoft.AspNetCore.Authentication;
using Telemetry.Ingress.API.Infrastructure.Auth;
using Telemetry.Ingress.API.Infrastructure.Options;

namespace Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;

public static class AuthExtensions
{
    extension (IServiceCollection services)
    {
        public AuthenticationBuilder ConfigureAuthentication()
        {
            return services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                    options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                })
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                    ApiKeyAuthenticationOptions.DefaultScheme,
                    options => { });
        }
    }
}
