using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Telemetry.Ingress.API.Infrastructure.Observability.Otel;

public static class OtelExtensions
{
    private const string ServiceName = "Telemetry.Ingress";
    private const string ActivitySourceName = "Telemetry.Ingress.Tracing";

    extension (IServiceCollection services)
    {
        public IServiceCollection ConfigureOpenTelemetry()
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(ServiceName))
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddMeter(IngressMetrics.MeterName)
                        .AddOtlpExporter();
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddSource(ActivitySourceName)
                        .AddOtlpExporter();
                });

            return services;
        }
    }
}
