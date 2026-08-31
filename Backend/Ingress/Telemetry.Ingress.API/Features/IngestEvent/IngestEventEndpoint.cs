using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Telemetry.Contracts.Constants;
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
                ClaimsPrincipal user,
                [FromServices] IngressMetrics metrics,
                [FromServices] LocalBufferService buffer,
                [FromServices] TimeProvider timeProvider) =>
            {
                if (!requestBody.IsValid())
                {
                    return Results.UnprocessableEntity();
                }

                var projectIdClaim = user.FindFirst(ClaimsConstants.ProjectId)?.Value;
                if (!Guid.TryParse(projectIdClaim, out var projectId))
                {
                    return Results.Unauthorized();
                }

                string? traceParent = Activity.Current?.Id;
                var receivedAt = timeProvider.GetUtcNow().UtcDateTime;

                var envelope = new EnvelopedEvent(
                    ProjectId: projectId,
                    Payload: requestBody,
                    TraceParent: traceParent,
                    ReceivedAt: receivedAt
                );

                var key = Ulid.NewUlid().ToByteArray();
                var value = JsonSerializer.SerializeToUtf8Bytes(envelope, IngressJsonContext.Default.EnvelopedEvent);

                using (var activity = ActivitySource.StartActivity("RocksDB Put", ActivityKind.Internal))
                {
                    buffer.Put(key, value);
                }

                metrics.RecordEventsReceived();

                return Results.Accepted();
            })
                .WithName("IngestTelemetryEvent")
                .RequireAuthorization()
                .Produces(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status503ServiceUnavailable);
        }
    }
}
