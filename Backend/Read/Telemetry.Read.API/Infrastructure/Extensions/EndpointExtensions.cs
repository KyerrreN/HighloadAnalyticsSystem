using System.Reflection;
using Telemetry.Read.API.Infrastructure.Endpoints;

namespace Telemetry.Read.API.Infrastructure.Extensions;

public static class EndpointExtensions
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapEndpoints(Assembly assembly)
        {
            var endpointTypes = assembly.GetTypes()
                .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            var endpoints = endpointTypes
                .Select(Activator.CreateInstance)
                .Cast<IEndpoint>()
                .ToList();

            var groupedEndpoints = endpoints.GroupBy(e => new { e.Version, e.Group });

            foreach (var group in groupedEndpoints)
            {
                // todo: fuck we need that for?
                var versionPrefix = group.Key.Version.StartsWith('v') ? group.Key.Version : $"v{group.Key.Version}";

                var groupRoutePattern = $"/api/{versionPrefix}/{group.Key.Group}";

                var routeGroupBuilder = app.MapGroup(groupRoutePattern);

                foreach (var endpoint in group)
                {
                    endpoint.MapEndpoint(routeGroupBuilder);
                }
            }

            return app;
        }
    }
}
