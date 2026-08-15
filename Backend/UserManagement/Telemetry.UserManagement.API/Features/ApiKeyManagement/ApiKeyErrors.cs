using Telemetry.UserManagement.Infrastructure.Result;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement;

public static class ApiKeyErrors
{
    public static readonly Error ProjectNotFound = new("ApiKey.ProjectNotFound", "Project was not found or access denied.");
    public static readonly Error CreationFailed = new("ApiKey.CreationFailed", "An error occurred while generating the API Key.");
    public static readonly Error FetchFailed = new("ApiKey.FetchFailed", "An error occurred while retrieving API keys.");
    public static readonly Error NotFound = new("ApiKey.NotFound", "API Key was not found or access denied.");
    public static readonly Error RevokeFailed = new("ApiKey.RevokeFailed", "An error occurred while revoking the API Key.");
}
