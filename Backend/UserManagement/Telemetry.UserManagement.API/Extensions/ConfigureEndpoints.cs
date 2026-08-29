using Telemetry.UserManagement.API.Features.ApiKeyManagement;
using Telemetry.UserManagement.API.Features.ApiKeyManagement.ValidateApiKey;
using Telemetry.UserManagement.API.Features.ProjectManagement;
using Telemetry.UserManagement.API.Features.UserManagement;

namespace Telemetry.UserManagement.API.Extensions;

public static class ConfigureEndpoints
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapAllEndpoints()
        {
            endpoints.MapUserManagementEndpoints();
            endpoints.MapProjectManagementEndpoints();
            endpoints.MapApiKeyManagementEndpoints();

            endpoints.MapGrpcService<ApiKeyValidationGrpcService>();

            return endpoints;
        }
    }
}
