using Telemetry.UserManagement.Infrastructure.Result;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement;

public static class ApiKeyErrors
{
    public static readonly Error EmptyName = new("ApiKey.EmptyName", "API Key name cannot be empty.");
    public static readonly Error ProjectNotFound = new("ApiKey.ProjectNotFound", "Project was not found or access denied.");
    public static readonly Error CreationFailed = new("ApiKey.CreationFailed", "An error occurred while generating the API Key.");
}
