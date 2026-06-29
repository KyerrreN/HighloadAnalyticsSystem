using Telemetry.Read.API.Features.GetDailyMontlyActiveUsers.Data;
using Telemetry.Read.API.Features.GetEventsVolume.Data;
using Telemetry.Read.API.Features.GetTopEvents.Data;

namespace Telemetry.Read.API.Infrastructure.Extensions;

public static class RegisterCustomServicesExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection RegisterCustomServices()
        {
            services.AddScoped<IDauMauDataSource, ClickHouseDauMauDataSource>();
            services.AddScoped<ITopEventsDataSource, ClickHouseTopEventsDataSource>();
            services.AddScoped<IEventsVolumeDataSource, ClickHouseEventsVolumeDataSource>();

            return services;
        }
    }
}
