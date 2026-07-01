using Telemetry.Ingress.API.Infrastructure.Observability.HighPerformanceLogging;

namespace Telemetry.Ingress.API.Infrastructure.Logging;

public static partial class KafkaLogging
{
    [LoggerMessage(
        EventId = LoggingEventIdConstants.KafkaLogStarted,
        Level = LogLevel.Information,
        Message = "Kafka topic is not initialized. Initializing...")]
    public static partial void LogTopicNotExist(this ILogger logger);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.KafkaLogTopicCreatedOrExists,
        Level = LogLevel.Information,
        Message = "Kafka topic \"{topicName}\" has been successfully created, or it already exists")]
    public static partial void LogTopicCreatedOrExists(this ILogger logger, string topicName);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.KafkaLogTopicAlreadyExists,
        Level = LogLevel.Information,
        Message = "Kafka topic \"{topicName}\" already exists")]
    public static partial void LogTopicAlreadyExists(this ILogger logger, string topicName);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.KafkaLogDeliveryError,
        Level = LogLevel.Error,
        Message = "Couldn't deliver message to Kafka. Reason: {reason}")]
    public static partial void LogDeliveryError(this ILogger logger, string reason);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.KafkaLogTopicUnknownCreationError,
        Level = LogLevel.Error,
        Message = "Unknown error while creating Kafka topic \"{topicName}\"")]
    public static partial void LogTopicUnknownCreationError(this ILogger logger, string topicName, Exception ex);
}
