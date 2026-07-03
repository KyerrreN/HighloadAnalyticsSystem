using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.MessageProcessing;
using Telemetry.Ingress.API.Infrastructure.Services;

namespace Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;

public static class Services
{
    private const string DefaultDbLocation = "wal_buffer_data";

    extension (IServiceCollection services)
    {
        public IServiceCollection RegisterServices()
        {
            services.AddSingleton<IEventMessageBus, KafkaEventMessageBus>();

            services.AddHostedService<TelemetryPublishWorker>();
            services.AddHostedService<SetupKafkaService>();

            return services;
        }

        public IServiceCollection RegisterRocksDb(IConfiguration configuration)
        {
            services.AddSingleton<LocalBufferService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("RocksDb");

                string dbPath = string.IsNullOrEmpty(connectionString)
                    ? DefaultDbLocation
                    : connectionString;

                return new LocalBufferService(dbPath);
            });

            return services;
        }
    }
}
