using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.MessageProcessing;

namespace Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;

public static class Services
{
    extension (IServiceCollection services)
    {
        public void RegisterServices()
        {
            services.AddSingleton<IEventMessageBus, KafkaEventMessageBus>();
            services.AddSingleton<ITelemetryEventChannel, TelemetryEventChannel>();

            services.AddHostedService<TelemetryPublishWorker>();
            services.AddHostedService<SetupKafkaService>();
        }
    }
}
