namespace Telemetry.Ingress.API.Infrastructure.Logging;

public static partial class HighPerformanceLogging
{
    // INFORMATION
    [LoggerMessage(
        EventId = 2000,
        Level = LogLevel.Information,
        Message = "Background worker for reading a channel has been started")]
    public static partial void LogBackgroundWorkerStarted(this ILogger logger);

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Worker's work has been cancelled")]
    public static partial void LogBackgroundWorkerCancelled(this ILogger logger);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Information,
        Message = "Kafka topic is not initialized. Initializing...")]
    public static partial void LogKafkaTopicNotExist(this ILogger logger);

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Information,
        Message = "Kafka topic \"{topicName}\" has been succesfully created, or it already exists")]
    public static partial void LogKafkaTopicSuccesfullyCreatedOrExists(this ILogger logger, string topicName);

    // ERROR
    [LoggerMessage(
        EventId = 4000,
        Level = LogLevel.Error,
        Message = "An error has occured while processing {substitute}")]
    public static partial void LogProcessingError(this ILogger logger, string substitute, Exception? ex);

    [LoggerMessage(
        EventId = 4001,
        Level = LogLevel.Error,
        Message = "Couldn't deliver message to Kafka. Reason: {reason}")]
    public static partial void LogKafkaDeliveryError(this ILogger logger, string reason);

    [LoggerMessage(
        EventId = 4002,
        Level = LogLevel.Error,
        Message = "Kafka topic \"{topicName}\" already exists")]
    public static partial void LogKafkaTopicAlreadyExist(this ILogger logger, string topicName);

    [LoggerMessage(
        EventId = 4003,
        Level = LogLevel.Error,
        Message = "Unknown error while creating Kafka topic \"{topicName}\"")]
    public static partial void LogKafkaTopicUnknownCreationError(this ILogger logger, string topicName, Exception e);
}
