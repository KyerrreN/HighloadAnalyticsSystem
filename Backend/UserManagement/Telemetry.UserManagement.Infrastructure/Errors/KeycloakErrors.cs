using Telemetry.Contracts.Result;

namespace Telemetry.UserManagement.Infrastructure.Errors;

public static class KeycloakErrors
{
    public static Error TokenRequestFailed(int statusCode) =>
        new("Keycloak.TokenRequestFailed", $"Failed to obtain admin token. Status code: {statusCode}");

    public static Error UserDeletionFailed(int statusCode) =>
        new("Keycloak.UserDeletionFailed", $"Failed to delete user from Keycloak. Status code: {statusCode}");

    public static readonly Error UnknownError =
        new("Keycloak.UnknownError", "An unexpected error occurred while communicating with Keycloak.");
}
