using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Telemetry.Contracts.Events;
using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.Observability.Otel;

namespace Telemetry.Ingress.API.Features.IngestEvent;

public static class IngestEventEndpoint
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapIngestEndpoints()
        {
            app.MapPost("events", async (
                [FromBody] TelemetryEvent requestBody,
                [FromServices] ITelemetryEventChannel channel,
                [FromServices] IngressMetrics metrics) =>
            {
                // todo: validation
                var activityContext = Activity.Current?.Context ?? default;
                var envelope = new EnvelopedEvent(requestBody, activityContext);

                var isWritten = channel.TryWrite(envelope);

                if (!isWritten)
                {
                    // kafka problems, channel overflow
                    metrics.RecordChannelRejected();
                    return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
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
