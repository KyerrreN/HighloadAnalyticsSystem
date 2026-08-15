using Telemetry.UserManagement.Infrastructure.Result;

namespace Telemetry.UserManagement.Infrastructure.Errors;

public static class UserErrors
{
    public static readonly Error NotFound = new("User.NotFound", "The user was not found.");
    public static readonly Error DeletionFailed = new("User.DeletionFailed", "An error occurred while deleting the user. No data was deleted.");
}
