namespace Telemetry.Read.API.Infrastructure.Endpoints;

public interface IEndpoint
{
    string Version { get; }

    string Group { get; }

    void MapEndpoint(IEndpointRouteBuilder app);
}
