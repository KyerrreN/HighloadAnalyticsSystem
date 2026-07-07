namespace Telemetry.Ingress.API.Infrastructure.Observability.HighPerformanceLogging;

public static partial class WorkerLogging
{
    [LoggerMessage(
        EventId = LoggingEventIdConstants.WorkerLogStarted,
        Level = LogLevel.Information,
        Message = "Background worker (buffer) has been started")]
    public static partial void LogStarted(this ILogger logger);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.WorkerLogCancelled,
        Level = LogLevel.Information,
        Message = "Worker's work has been cancelled")]
    public static partial void LogCancelled(this ILogger logger);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.WorkerLogProcessingError,
        Level = LogLevel.Error,
        Message = "An error occurred while processing {componentName}")]
    public static partial void LogProcessingError(this ILogger logger, string componentName, Exception ex);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.WorkerLogDeserializationError,
        Level = LogLevel.Error,
        Message = "Poison pill detected in WAL. Message corrupted and will be dropped.")]
    public static partial void LogWorkerDeserializationError(this ILogger logger, Exception? ex);
}
