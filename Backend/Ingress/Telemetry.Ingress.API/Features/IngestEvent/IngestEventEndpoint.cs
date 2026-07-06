using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using Telemetry.Contracts.Events;
using Telemetry.Ingress.API.Infrastructure.Observability.Otel;
using Telemetry.Ingress.API.Infrastructure.Services;

namespace Telemetry.Ingress.API.Features.IngestEvent;

public static class IngestEventEndpoint
{
    private static readonly ActivitySource ActivitySource = new(OtelConstants.ActivitySourceName);

    extension(IEndpointRouteBuilder app)
    {
        public void MapIngestEndpoints()
        {
            app.MapPost("events", (
                [FromBody] TelemetryEvent requestBody,
                [FromServices] IngressMetrics metrics,
                [FromServices] LocalBufferService buffer) =>
            {
                // todo: validation
                string? traceParent = Activity.Current?.Id;
                var envelope = new EnvelopedEvent(requestBody, traceParent);

                var key = Ulid.NewUlid().ToByteArray();
                var value = JsonSerializer.SerializeToUtf8Bytes(envelope);

                using (var activity = ActivitySource.StartActivity("RocksDB Put", ActivityKind.Internal))
                {
                    buffer.Put(key, value);
                }

                metrics.RecordEventsReceived();

                return Results.Accepted();
            })
                .WithName("IngestTelemetryEvent")
                .Produces(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status503ServiceUnavailable);
        }
    }
}
