using Microsoft.AspNetCore.Mvc;
using Telemetry.Ingress.Domain.Events;
using Telemetry.Ingress.Domain.Interfaces;

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
            [FromServices] ITelemetryEventChannel channel)
        {
            // todo: validation
            var isWritten = channel.TryWrite(requestBody);

            if (!isWritten)
            {
                // kafka problems
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            return Results.Accepted();
        }
    }
}
