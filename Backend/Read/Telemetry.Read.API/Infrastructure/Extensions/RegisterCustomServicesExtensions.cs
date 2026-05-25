using Telemetry.Read.API.Features.GetDailyMontlyActiveUsers.Data;

namespace Telemetry.Read.API.Infrastructure.Extensions;

public static class RegisterCustomServicesExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection RegisterCustomServices()
        {
            services.AddScoped<IDauMauDataSource, ClickHouseDauMauDataSource>();

            return services;
        }
    }
}
