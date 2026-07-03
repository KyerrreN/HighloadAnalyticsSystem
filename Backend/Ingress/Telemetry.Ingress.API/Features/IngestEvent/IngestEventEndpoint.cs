using Microsoft.AspNetCore.Mvc;
using RocksDbSharp;
using System.Diagnostics;
using System.Text.Json;
using Telemetry.Contracts.Events;
using Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;
using Telemetry.Ingress.API.Infrastructure.Observability.Otel;

namespace Telemetry.Ingress.API.Features.IngestEvent;

public static class IngestEventEndpoint
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapIngestEndpoints()
        {
            app.MapPost("events", (
                [FromBody] TelemetryEvent requestBody,
                [FromServices] IngressMetrics metrics,
                [FromServices] RocksDb db) =>
            {
                // todo: validation
                var activityContext = Activity.Current?.Context ?? default;
                var envelope = new EnvelopedEvent(requestBody, activityContext);

                var key = Ulid.NewUlid().ToByteArray();
                var value = JsonSerializer.SerializeToUtf8Bytes(envelope);

                db.Put(key, value, writeOptions: RocksDbDefaults.AsyncWriteOptions);

                metrics.RecordEventsReceived();

                return Results.Accepted();
            })
                .WithName("IngestTelemetryEvent")
                .Produces(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status503ServiceUnavailable);
        }
    }
}
