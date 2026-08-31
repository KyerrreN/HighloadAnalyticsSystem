using Microsoft.Extensions.Logging;

namespace Telemetry.UserManagement.Infrastructure.Logging;

public static partial class ApiKeyLogs
{
    [LoggerMessage(
        EventId = LogEventIds.ApiKey.ApiKeyFetchFailed,
        Level = LogLevel.Error,
        Message = "Failed to fetch API keys for project {ProjectId}")]
    public static partial void LogApiKeyFetchFailed(
        this ILogger logger,
        Exception exception,
        Guid projectId);

    [LoggerMessage(
        EventId = LogEventIds.ApiKey.ApiKeyCreationFailed,
        Level = LogLevel.Error,
        Message = "Failed to create API key {KeyName} for project {ProjectId}")]
    public static partial void LogApiKeyCreationFailed(
        this ILogger logger,
        Exception exception,
        string keyName,
        Guid projectId);

    [LoggerMessage(
        EventId = LogEventIds.ApiKey.ApiKeyRevokeFailed,
        Level = LogLevel.Error,
        Message = "Failed to revoke API key {KeyId} for project {ProjectId}")]
    public static partial void LogApiKeyRevokeFailed(
        this ILogger logger,
        Exception exception,
        Guid keyId,
        Guid projectId);

    [LoggerMessage(
        EventId = LogEventIds.ApiKey.ApiKeyValidationError,
        Level = LogLevel.Error,
        Message = "Failed to validate API key hash.")]
    public static partial void LogApiKeyValidationError(
        this ILogger logger,
        Exception exception);

    [LoggerMessage(
        EventId = LogEventIds.ApiKey.ApiKeyValidationWarning,
        Level = LogLevel.Warning,
        Message = "gRPC API Key validation failed: {ErrorCode} - {ErrorMessage}")]
    public static partial void LogApiKeyValidationWarning(
        this ILogger logger,
        string errorCode,
        string errorMessage);
}
