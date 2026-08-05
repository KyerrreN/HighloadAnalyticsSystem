using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telemetry.UserManagement.Infrastructure.Database;
using Telemetry.UserManagement.Infrastructure.Database.Options;

namespace Telemetry.UserManagement.Infrastructure.DI;

public static class AddInfrastructureExtension
{
    extension (IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.Configure<PostgresOptions>(configuration.GetSection(PostgresOptions.SectionName));

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var postgresOptions = sp.GetRequiredService<IOptions<PostgresOptions>>().Value;

                options.UseNpgsql(postgresOptions.ConnectionString, opt =>
                {
                    opt.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                });
            });

            return services;
        }
    }
}
