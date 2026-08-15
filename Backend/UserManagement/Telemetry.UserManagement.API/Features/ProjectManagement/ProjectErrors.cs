using Telemetry.UserManagement.Infrastructure.Result;

namespace Telemetry.UserManagement.API.Features.ProjectManagement;

public static class ProjectErrors
{
    public static readonly Error EmptyName = new("Project.EmptyName", "Project name cannot be empty.");
    public static readonly Error AlreadyExists = new("Project.AlreadyExists", "A project with this name already exists for this user.");
    public static readonly Error CreationFailed = new("Project.CreationFailed", "An error occurred while creating the project.");
    public static readonly Error FetchFailed = new("Project.FetchFailed", "An error occurred while retrieving projects.");
    public static readonly Error NotFound = new("Project.NotFound", "The requested project was not found.");
    public static readonly Error DeleteFailed = new("Project.DeleteFailed", "An error occurred while deleting the project.");
}
