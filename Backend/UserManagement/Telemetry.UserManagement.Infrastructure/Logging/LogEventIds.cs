namespace Telemetry.UserManagement.Infrastructure.Logging;

public static class LogEventIds
{
    public static class User
    {
        public const int UserDeletionFailed = 1001;
    }

    public static class Project
    {
        public const int ProjectCreationFailed = 2001;
        public const int ProjectFetchFailed = 2002;
        public const int ProjectByIdFetchFailed = 2003;
        public const int ProjectDeletionFailed = 2004;
    }

    public static class ApiKey
    {
        public const int ApiKeyCreationFailed = 3001;
        public const int ApiKeyFetchFailed = 3002;
        public const int ApiKeyRevokeFailed = 3003;
    }
}
