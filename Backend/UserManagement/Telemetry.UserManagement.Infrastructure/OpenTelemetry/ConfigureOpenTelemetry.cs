using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Telemetry.Shared.Observability;

namespace Telemetry.UserManagement.Infrastructure.OpenTelemetry;

public static class ConfigureOpenTelemetry
{
    extension (IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureOtel()
        {
            builder.ConfigureOpenTelemetry(
                serviceName: "telemetry-user-management-api",
                configureMetrics: metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation();
                },
                configureTracing: tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = true;
                        })
                        .AddNpgsql();
                });

            return builder;
        }
    }
}
