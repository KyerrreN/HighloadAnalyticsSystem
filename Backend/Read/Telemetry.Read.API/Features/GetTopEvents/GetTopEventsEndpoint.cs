using Telemetry.Read.API.Infrastructure.Endpoints;
using Telemetry.Read.CrossCuttingConcerns.Abstractions;

namespace Telemetry.Read.API.Features.GetTopEvents;

public class GetTopEventsEndpoint : IEndpoint
{
    public string Version => "1";

    public string Group => "analytics";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("top-events", async (
            [AsParameters] GetTopEventsQuery query,
            IQueryHandler<GetTopEventsQuery, GetTopEventsResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.HandleAsync(query, cancellationToken);

            return Results.Ok(result);
        })
            .WithName("GetTopEventsInPeriod");
    }
}
