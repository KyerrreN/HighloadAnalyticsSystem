using Telemetry.UserManagement.API.Features.ProjectManagement.CreateProject;
using Telemetry.UserManagement.API.Features.ProjectManagement.DeleteProject;
using Telemetry.UserManagement.API.Features.ProjectManagement.GetAllProjects;
using Telemetry.UserManagement.API.Features.ProjectManagement.GetProjectById;

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
            group.MapGetAllProjectsEndpoint();
            group.MapGetProjectByIdEndpoint();
            group.MapDeleteProjectEndpoint();

            return endpoints;
        }
    }
}
