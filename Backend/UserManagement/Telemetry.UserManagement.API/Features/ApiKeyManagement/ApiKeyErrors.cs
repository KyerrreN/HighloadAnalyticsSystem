using Telemetry.Contracts.Result;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement;

public static class ApiKeyErrors
{
    public static readonly Error ProjectNotFound = new("ApiKey.ProjectNotFound", "Project was not found or access denied.");
    public static readonly Error CreationFailed = new("ApiKey.CreationFailed", "An error occurred while generating the API Key.");
    public static readonly Error FetchFailed = new("ApiKey.FetchFailed", "An error occurred while retrieving API keys.");
    public static readonly Error NotFound = new("ApiKey.NotFound", "API Key was not found or access denied.");
    public static readonly Error RevokeFailed = new("ApiKey.RevokeFailed", "An error occurred while revoking the API Key.");
    public static readonly Error InvalidHash = new("ApiKey.InvalidHash", "Provided key hash is invalid or empty.");
    public static readonly Error InvalidOrExpired = new("ApiKey.InvalidOrExpired", "API key is invalid, revoked, expired, or project is deleted.");
    public static readonly Error ValidationFailed = new("ApiKey.ValidationFailed", "An unexpected error occurred during API key validation.");
}
