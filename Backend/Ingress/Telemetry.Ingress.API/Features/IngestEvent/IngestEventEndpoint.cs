using Microsoft.AspNetCore.Mvc;
using Telemetry.Contracts.Events;
using Telemetry.Contracts.Interfaces;
using Telemetry.Ingress.API.Infrastructure.Observability;

namespace Telemetry.Ingress.API.Features.IngestEvent;

public static class IngestEventEndpoint
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapIngestEndpoints()
        {
            app.MapPost("events", ProcessEvent)
                .WithName("IngestTelemetryEvent")
                .Produces(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status503ServiceUnavailable);
        }

        private static IResult ProcessEvent(
            [FromBody] TelemetryEvent requestBody,
            [FromServices] ITelemetryEventChannel channel,
            [FromServices] IngressMetrics metrics)
        {
            // todo: validation
            var isWritten = channel.TryWrite(requestBody);

            if (!isWritten)
            {
                // kafka problems
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            metrics.EventsReceivedCounter.Add(1, new KeyValuePair<string, object?>("project_key", requestBody.ProjectApiKey));

            return Results.Accepted();
        }
    }
}
