using Asp.Versioning;
using Telemetry.Ingress.API.Features.IngestEvent;

namespace Telemetry.Ingress.API.Infrastructure.Endpoints;

public static class EndpointRegistation
{
    extension (IEndpointRouteBuilder app)
    {
        public void MapEndpoints()
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            var group = app
                .MapGroup("api/v{version:apiVersion}")
                .WithApiVersionSet(apiVersionSet);

            group.MapIngestEndpoints();
        }
    }
}
