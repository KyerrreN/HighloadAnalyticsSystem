using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Telemetry.Read.Domain.OpenTelemetry;
using Telemetry.Shared.Observability;

namespace Telemetry.Read.API.Infrastructure.Observability;

public static class OtelExtensions
{
    private const string ServiceName = "Telemetry.Read";

    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureOtel()
        {
            builder.Services.AddSingleton<ReadApiMetrics>();

            builder.ConfigureOpenTelemetry(
                serviceName: ServiceName,
                configureTracing: (tracing) =>
                {
                    tracing
                        .AddSource(OtelConstants.ActivitySourceName)
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation();
                },
                configureMetrics: (metrics) =>
                {
                    metrics
                        .AddMeter(ReadApiMetrics.MeterName)
                        .AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                });

            return builder;
        }
    }
}
