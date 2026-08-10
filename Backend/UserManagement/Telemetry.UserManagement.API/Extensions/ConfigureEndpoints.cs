using Telemetry.UserManagement.API.Features.UserManagement;

namespace Telemetry.UserManagement.API.Extensions;

public static class ConfigureEndpoints
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapAllEndpoints()
        {
            endpoints.MapUserManagementEndpoints();

            return endpoints;
        }
    }
}
