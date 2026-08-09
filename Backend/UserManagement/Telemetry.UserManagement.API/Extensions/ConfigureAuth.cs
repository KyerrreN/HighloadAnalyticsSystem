using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Telemetry.UserManagement.API.Extensions;

public static class ConfigureAuth
{
    extension(IServiceCollection services)
    {
        public IServiceCollection ConfigureAuthentication(IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var authority = configuration["Keycloak:Authority"]; // todo: strongly typed
                    var requireHttps = configuration.GetValue<bool>("Keycloak:RequireHttpsMetadata"); // todo: strongly typed

                    options.Authority = authority;
                    options.RequireHttpsMetadata = requireHttps;

                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false, // todo: toggle
                        NameClaimType = "preferred_username",
                        RoleClaimType = "roles"
                    };
                });

            return services;
        }
    }
}
