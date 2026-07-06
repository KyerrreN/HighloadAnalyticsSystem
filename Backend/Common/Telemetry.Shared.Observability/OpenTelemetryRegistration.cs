using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Telemetry.Shared.Observability;

public static class OpenTelemetryRegistration
{
    extension (IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureOpenTelemetry(
            string serviceName,
            Action<MeterProviderBuilder>? configureMetrics = null,
            Action<TracerProviderBuilder>? configureTracing = null)
        {
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(serviceName);

            ConfigureLogging(builder, resourceBuilder);
            ConfigureMetrics(builder, resourceBuilder, configureMetrics);
            ConfigureTracing(builder, resourceBuilder, configureTracing);

            return builder;
        }
    }

    private static void ConfigureLogging(IHostApplicationBuilder builder, ResourceBuilder resourceBuilder)
    {
        builder.Logging.ClearProviders();

        if (builder.Environment.IsDevelopment())
        {
            builder.Logging.AddConsole();
        }

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.SetResourceBuilder(resourceBuilder);

            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.AddOtlpExporter();
        });
    }

    private static void ConfigureMetrics(
        IHostApplicationBuilder builder, 
        ResourceBuilder resourceBuilder, 
        Action<MeterProviderBuilder>? configureMetrics)
    {
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation();

                configureMetrics?.Invoke(metrics);

                metrics.AddOtlpExporter();
            });
    }

    private static void ConfigureTracing(
        IHostApplicationBuilder builder,
        ResourceBuilder resourceBuilder,
        Action<TracerProviderBuilder>? configureTracing)
    {
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .AddHttpClientInstrumentation();

                configureTracing?.Invoke(tracing);

                tracing.AddOtlpExporter();
            });
    }
}
