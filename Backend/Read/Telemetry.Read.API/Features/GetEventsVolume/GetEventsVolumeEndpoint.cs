using Telemetry.Read.API.Infrastructure.Endpoints;
using Telemetry.Read.CrossCuttingConcerns.Abstractions;

namespace Telemetry.Read.API.Features.GetEventsVolume;

public class GetEventsVolumeEndpoint : IEndpoint
{
    public string Version => "1";

    public string Group => "analytics";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("event-volume", async (
            [AsParameters] GetEventsVolumeQuery query,
            IQueryHandler<GetEventsVolumeQuery, GetEventsVolumeResponse> queryHandler,
            CancellationToken ct) =>
        {
            var result = await queryHandler.HandleAsync(query, ct);

            return Results.Ok(result);
        })
            .WithName("GetEventsVolume");
    }
}
