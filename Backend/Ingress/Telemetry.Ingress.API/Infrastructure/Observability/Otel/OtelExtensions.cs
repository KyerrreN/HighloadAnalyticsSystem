using Microsoft.AspNetCore.HttpLogging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Telemetry.Ingress.API.Infrastructure.Observability.Otel;

public static class OtelExtensions
{
    private const string ServiceName = "Telemetry.Ingress";
    private const string ActivitySourceName = "Telemetry.Ingress.Tracing";

    extension (WebApplicationBuilder builder)
    {
        public WebApplicationBuilder ConfigureOpenTelemetry()
        {
            builder.Logging.ClearProviders();
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddConsole();
            }

            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName));

                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
                logging.AddOtlpExporter();
            });

            builder.Services.AddOpenTelemetry()
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
