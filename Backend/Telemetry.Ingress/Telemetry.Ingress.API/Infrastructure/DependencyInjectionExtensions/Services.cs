using Telemetry.Ingress.API.Infrastructure.MessageProcessing;
using Telemetry.Ingress.Domain.Interfaces;

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
