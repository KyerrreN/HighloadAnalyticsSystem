using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Telemetry.Read.Domain.Options;

namespace Telemetry.Read.API.Infrastructure.Extensions;

public static class AuthExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection ConfigureAuthentication(IConfiguration configuration)
        {
            var keycloakOptions = configuration
                .GetSection(KeycloakOptions.SectionName)
                .Get<KeycloakOptions>()
                ?? throw new InvalidOperationException($"Section '{KeycloakOptions.SectionName}' is missing.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = keycloakOptions.Authority;
                    options.RequireHttpsMetadata = keycloakOptions.RequireHttpsMetadata;

                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudience = keycloakOptions.Audience,

                        ValidateIssuer = true,
                        ValidIssuer = keycloakOptions.Authority,

                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        RequireSignedTokens = true,

                        NameClaimType = "preferred_username",
                        RoleClaimType = "roles"
                    };
                });

            return services;
        }
    }
}
