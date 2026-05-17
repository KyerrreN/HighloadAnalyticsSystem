using Telemetry.Ingress.API.Features.IngestEvent;

namespace Telemetry.Ingress.API.Infrastructure;

public static class EndpointRegistation
{
    extension (IEndpointRouteBuilder app)
    {
        // todo: perhpas move it to autoregistration
        public void MapEndpoints()
        {
            var group = app.MapGroup("api");

            group.MapIngestEndpoints();
        }
    }
}
