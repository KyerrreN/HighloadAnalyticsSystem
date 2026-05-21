using Telemetry.Read.API.Infrastructure.Endpoints;
using Telemetry.Read.CrossCuttingConcerns.Abstractions;

namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers;

public class GetDauMauEndpoint : IEndpoint
{
    public string Version => "1";

    public string Group => "analytics";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dau", async (
            [AsParameters] GetDauMauQuery query,
            IQueryHandler<GetDauMauQuery, List<DauMauResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(query, cancellationToken);

            return Results.Ok(result);
        })
            .WithName("GetDailyActiveUsers");
    }
}
