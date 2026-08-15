using Telemetry.UserManagement.API.Features.ApiKeyManagement.CreateApiKey;
using Telemetry.UserManagement.API.Features.ApiKeyManagement.GetApiKeysForProject;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement;

public static class ApiKeyManagementEndpoints
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapApiKeyManagementEndpoints()
        {
            var group = endpoints
                .MapGroup("/api/projects")
                .WithTags("ApiKeys")
                .RequireAuthorization();

            group.MapCreateApiKeyEndpoint();
            group.MapGetApiKeysForProjectEndpoint();

            return endpoints;
        }
    }
}
