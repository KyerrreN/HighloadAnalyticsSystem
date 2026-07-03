using RocksDbSharp;
using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.MessageProcessing;

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
            services.AddSingleton<RocksDb>(sp =>
            {
                var dbPath = DefaultDbLocation;

                if (!string.IsNullOrEmpty(configuration.GetConnectionString("RocksDb")))
                {
                    dbPath = configuration.GetConnectionString("RocksDb");
                }
                var opt = new DbOptions().SetCreateIfMissing(true);

                return RocksDb.Open(opt, dbPath);
            });

            return services;
        }
    }
}
