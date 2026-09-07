using Microsoft.OpenApi;

namespace Telemetry.Read.API.Infrastructure.Extensions;

public static class SwaggerExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection ConfigureSwagger()
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Read API", Version = "v1" });

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT"
                };

                options.AddSecurityDefinition("bearer", jwtSecurityScheme);

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = []
                });
            });

            return services;
        }
    }
}
