using Telemetry.Shared.Observability;

namespace Telemetry.Worker.Infrastructure.Observability.Otel;

public static class OtelExtensions
{
    private const string ServiceName = "Telemetry.Worker";

    extension (IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureOtel()
        {
            builder.Services.AddSingleton<WorkerMetrics>();
            builder.ConfigureOpenTelemetry(
                serviceName: ServiceName,
                configureMetrics: (metrics) =>
                {
                    metrics.AddMeter(WorkerMetrics.MeterName);
                },
                configureTracing: (tracing) =>
                {
                    tracing.AddSource(OtelConstants.TelemetrySinkActivitySourceName);
                });

            return builder;
        }
    }
}
