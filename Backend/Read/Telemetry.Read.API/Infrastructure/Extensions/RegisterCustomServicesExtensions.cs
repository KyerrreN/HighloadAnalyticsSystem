using Telemetry.Read.API.Features.Analytics.GetDailyMontlyActiveUsers.Data;
using Telemetry.Read.API.Features.Analytics.GetEventsVolume.Data;
using Telemetry.Read.API.Features.Analytics.GetTopEvents.Data;
using Telemetry.Read.Domain.Services;

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
            services.AddScoped<IProjectAccessService, ProjectAccessService>();

            return services;
        }
    }
}
