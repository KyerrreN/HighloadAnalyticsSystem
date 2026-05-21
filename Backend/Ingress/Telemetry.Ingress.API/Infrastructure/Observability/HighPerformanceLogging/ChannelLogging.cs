namespace Telemetry.Ingress.API.Infrastructure.Observability.HighPerformanceLogging;

public static partial class ChannelLogging
{
    [LoggerMessage(
        EventId = LoggingEventIdConstants.ChannelLogStarted,
        Level = LogLevel.Information,
        Message = "Background worker for reading a channel has been started")]
    public static partial void LogStarted(this ILogger logger);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.ChannelLogCancelled,
        Level = LogLevel.Information,
        Message = "Worker's work has been cancelled")]
    public static partial void LogCancelled(this ILogger logger);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.ChannelLogProcessingError,
        Level = LogLevel.Error,
        Message = "An error occurred while processing {componentName}")]
    public static partial void LogProcessingError(this ILogger logger, string componentName, Exception ex);
}
