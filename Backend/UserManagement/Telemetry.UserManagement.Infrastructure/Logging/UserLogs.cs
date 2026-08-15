using Microsoft.Extensions.Logging;

namespace Telemetry.UserManagement.Infrastructure.Logging;

public static partial class UserLogs
{
    [LoggerMessage(
        EventId = LogEventIds.User.UserDeletionFailed,
        Level = LogLevel.Error,
        Message = "A database or unexpected error occurred while deleting user {UserId}")]
    public static partial void LogUserDeletionFailed(
        this ILogger logger,
        Exception exception,
        Guid userId);
}
