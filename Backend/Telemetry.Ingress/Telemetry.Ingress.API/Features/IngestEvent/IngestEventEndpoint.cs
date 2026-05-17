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
            // todo: extract later. perhaps think of auto registration
            app.MapPost("events", ProcessEvent)
                .WithName("IngestTelemetryEvent")
                .Produces(StatusCodes.Status202Accepted);
        }

        private static async Task<IResult> ProcessEvent(
            [FromBody] TelemetryEvent requestBody,
            IEventMessageBus messageBus,
            CancellationToken cancellationToken = default)
        {
            // todo: validation

            await messageBus.PublishAsync(requestBody, cancellationToken);

            return Results.Accepted();
        }
    }
}
