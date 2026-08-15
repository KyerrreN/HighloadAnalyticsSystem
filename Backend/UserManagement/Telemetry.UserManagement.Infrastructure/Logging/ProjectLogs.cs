using Microsoft.Extensions.Logging;

namespace Telemetry.UserManagement.Infrastructure.Logging;

public static partial class ProjectLogs
{
    [LoggerMessage(
        EventId = LogEventIds.Project.ProjectCreationFailed,
        Level = LogLevel.Error,
        Message = "Failed to create project {ProjectName} for user {UserId}")]
    public static partial void LogProjectCreationFailed(
        this ILogger logger,
        Exception exception,
        string projectName,
        Guid userId);

    [LoggerMessage(
        EventId = LogEventIds.Project.ProjectFetchFailed,
        Level = LogLevel.Error,
        Message = "Failed to fetch projects for user {UserId}")]
    public static partial void LogProjectFetchFailed(
        this ILogger logger,
        Exception exception,
        Guid userId);

    [LoggerMessage(
        EventId = LogEventIds.Project.ProjectByIdFetchFailed,
        Level = LogLevel.Error,
        Message = "Failed to fetch project {ProjectId} for user {UserId}")]
    public static partial void LogProjectByIdFetchFailed(
        this ILogger logger,
        Exception exception,
        Guid projectId,
        Guid userId);

    [LoggerMessage(
        EventId = LogEventIds.Project.ProjectDeletionFailed,
        Level = LogLevel.Error,
        Message = "Failed to delete project {ProjectId} for user {UserId}")]
    public static partial void LogProjectDeletionFailed(
        this ILogger logger,
        Exception exception,
        Guid projectId,
        Guid userId);
}
