using Telemetry.UserManagement.API.Features.ApiKeyManagement.CreateApiKey;
using Telemetry.UserManagement.API.Features.ApiKeyManagement.GetApiKeysForProject;
using Telemetry.UserManagement.API.Features.ApiKeyManagement.RevokeApiKey;

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
            group.MapRevokeApiKeyEndpoint();

            return endpoints;
        }
    }
}
