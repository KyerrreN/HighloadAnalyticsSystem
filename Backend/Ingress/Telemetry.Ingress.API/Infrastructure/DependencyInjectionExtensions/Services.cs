using Microsoft.Extensions.Options;
using Telemetry.Contracts.Interfaces;
using Telemetry.Contracts.Utils;
using Telemetry.Ingress.API.Features.ApiKeys;
using Telemetry.Ingress.API.Features.SinkToKafka;
using Telemetry.Ingress.API.Infrastructure.MessageProcessing;
using Telemetry.Ingress.API.Infrastructure.Options;
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
            services.AddSingleton<IApiKeyHasher, ApiKeyHasher>();

            services.AddScoped<IApiKeyCacheService, ApiKeyCacheService>();

            services.AddHostedService<TelemetryPublishWorker>();
            services.AddHostedService<SetupKafkaService>();

            return services;
        }

        public IServiceCollection RegisterRocksDb(IConfiguration configuration)
        {
            services.Configure<RocksDbOptions>(configuration.GetSection(RocksDbOptions.SectionName));

            services.AddSingleton<LocalBufferService>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RocksDbOptions>>().Value;

                string dbPath = string.IsNullOrEmpty(options.ConnectionString)
                    ? DefaultDbLocation
                    : options.ConnectionString;

                return new LocalBufferService(dbPath);
            });

            services.AddSingleton<IApiKeyHasher, ApiKeyHasher>();

            return services;
        }
    }
}
