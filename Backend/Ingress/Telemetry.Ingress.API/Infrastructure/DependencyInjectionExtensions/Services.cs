using RocksDbSharp;
using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.MessageProcessing;

namespace Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;

public static class Services
{
    extension (IServiceCollection services)
    {
        public IServiceCollection RegisterServices()
        {
            services.AddSingleton<IEventMessageBus, KafkaEventMessageBus>();

            services.AddHostedService<TelemetryPublishWorker>();
            services.AddHostedService<SetupKafkaService>();

            return services;
        }

        public IServiceCollection RegisterRocksDb(string dbPath = "wal_buffer_data") // todo: config
        {
            services.AddSingleton<RocksDb>(sp =>
            {
                var opt = new DbOptions().SetCreateIfMissing(true);

                return RocksDb.Open(opt, dbPath);
            });

            return services;
        }
    }
}
