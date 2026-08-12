using Telemetry.UserManagement.API.Features.ProjectManagement.CreateProject;

namespace Telemetry.UserManagement.API.Features.ProjectManagement;

public static class ProjectManagementEndpoints
{
    extension (IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapProjectManagementEndpoints()
        {
            var group = endpoints.MapGroup("/api/projects")
                .WithTags("Project")
                .RequireAuthorization();

            group.MapCreateProjectEndpoint();

            return endpoints;
        }
    }
}
