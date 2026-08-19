using Microsoft.OpenApi;

namespace Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;

public static class SwaggerExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection ConfigureSwagger()
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Telemetry Ingress API", Version = "v1" });

                var apiKeyScheme = new OpenApiSecurityScheme
                {
                    Name = "X-API-Key",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "Enter API Key (e.g. tlm_test123)"
                };

                options.AddSecurityDefinition("ApiKey", apiKeyScheme);

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("ApiKey", document)] = []
                });
            });

            return services;
        }
    }
}
