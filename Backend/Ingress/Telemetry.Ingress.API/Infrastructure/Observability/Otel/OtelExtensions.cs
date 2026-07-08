using Microsoft.AspNetCore.HttpLogging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Telemetry.Shared.Observability;

namespace Telemetry.Ingress.API.Infrastructure.Observability.Otel;

public static class OtelExtensions
{
    private const string ServiceName = "Telemetry.Ingress";

    extension (WebApplicationBuilder builder)
    {
        public WebApplicationBuilder ConfigureOpenTelemetry()
        {
            builder.ConfigureOpenTelemetry(
                serviceName: ServiceName,
                configureMetrics: metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddMeter(IngressMetrics.MeterName);
                },
                configureTracing: tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddSource(OtelConstants.ActivitySourceName);
                });

            builder.Services.AddHttpLogging(opt =>
            {
                opt.LoggingFields = HttpLoggingFields.RequestPath
                                    | HttpLoggingFields.RequestMethod
                                    | HttpLoggingFields.ResponseStatusCode
                                    | HttpLoggingFields.Duration;

                opt.CombineLogs = true;
            });

            return builder;
        }
    }
}
