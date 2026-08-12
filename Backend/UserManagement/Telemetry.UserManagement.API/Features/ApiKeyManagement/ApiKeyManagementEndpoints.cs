using Telemetry.UserManagement.API.Features.ApiKeyManagement.CreateApiKey;

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

            return endpoints;
        }
    }
}
