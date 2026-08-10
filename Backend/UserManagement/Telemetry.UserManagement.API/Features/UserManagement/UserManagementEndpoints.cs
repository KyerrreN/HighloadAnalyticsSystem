using Telemetry.UserManagement.API.Features.UserManagement.DeleteUser;

namespace Telemetry.UserManagement.API.Features.UserManagement;

public static class UserManagementEndpoints
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapUserManagementEndpoints()
        {
            var group = endpoints.MapGroup("/api/users")
                .WithTags("User")
                .RequireAuthorization();

            group.MapDeleteUserEndpoint();

            return endpoints;
        }
    }
}
