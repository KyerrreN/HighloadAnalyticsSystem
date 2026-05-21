using Telemetry.Worker.Infrastructure.Constants;

namespace Telemetry.Worker.Infrastructure.Observability.Logging;

public static partial class WorkerLogging
{
    [LoggerMessage(
        EventId = LoggingConstants.PartitionsAssigned,
        Level = LogLevel.Information,
        Message = "Available partitions: {partitions}")]
    public static partial void LogPartitionsAssigned(this ILogger logger, string partitions);

    [LoggerMessage(
        EventId = LoggingConstants.SubscribedToTopic,
        Level = LogLevel.Information,
        Message = "Successfully subscribed to topic {topic}. Awaiting messages...")]
    public static partial void LogSubscribedToTopic(this ILogger logger, string topic);

    [LoggerMessage(
        EventId = LoggingConstants.FlushingBatch,
        Level = LogLevel.Information,
        Message = "Flushing batch of {count} messages. Reason: {reason}")]
    public static partial void LogFlushingBatch(this ILogger logger, int count, string reason);

    [LoggerMessage(
        EventId = LoggingConstants.BatchSaved,
        Level = LogLevel.Information,
        Message = "Batch successfully saved to sink and committed to Kafka")]
    public static partial void LogBatchSaved(this ILogger logger);

    [LoggerMessage(
        EventId = LoggingConstants.Stopping,
        Level = LogLevel.Information,
        Message = "Caught stopping signal. Stopping...")]
    public static partial void LogStopping(this ILogger logger);

    [LoggerMessage(
        EventId = LoggingConstants.Disconnected,
        Level = LogLevel.Information,
        Message = "Disconnected from Kafka")]
    public static partial void LogDisconnected(this ILogger logger);

    [LoggerMessage(
        EventId = LoggingConstants.DeserializationError,
        Level = LogLevel.Error,
        Message = "Error deserializing message from Kafka.")]
    public static partial void LogDeserializationError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = LoggingConstants.SinkFlushError,
        Level = LogLevel.Error,
        Message = "Failed to flush batch to sink. Retrying...")]
    public static partial void LogSinkFlushError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = LoggingConstants.InitializingClickHouse,
        Level = LogLevel.Information,
        Message = "Checking and initializing ClickHouse...")]
    public static partial void LogInitializingClickHouse(this ILogger logger);

    [LoggerMessage(
        EventId = LoggingConstants.ClickHouseInitialized,
        Level = LogLevel.Information,
        Message = "ClickHouse's schema is initialized successfully")]
    public static partial void LogClickHouseInitialized(this ILogger logger);

    [LoggerMessage(
        EventId = LoggingConstants.ClickHouseInitializationFailed,
        Level = LogLevel.Critical,
        Message = "FATAL: Couldn't initialize ClickHouse schema. Check DB Connection or ConnectionString")]
    public static partial void LogClickHouseInitializationFailed(this ILogger logger, Exception ex);
}
