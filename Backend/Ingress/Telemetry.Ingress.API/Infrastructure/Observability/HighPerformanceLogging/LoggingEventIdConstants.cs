namespace Telemetry.Ingress.API.Infrastructure.Observability.HighPerformanceLogging;

public static class LoggingEventIdConstants
{
    // Kafka (X100-X199)
    // Information (2100-2199)
    public const int KafkaLogStarted = 2100;
    public const int KafkaLogTopicCreatedOrExists = 2101;
    public const int KafkaLogTopicAlreadyExists = 2102;

    // Error (4100-4199)
    public const int KafkaLogDeliveryError = 4100;
    public const int KafkaLogTopicUnknownCreationError = 4101;
    public const int KafkaLogMessageRejected = 4102;

    // Worker (X000-X099)
    // Information (2000-2099)
    public const int WorkerLogStarted = 2000;
    public const int WorkerLogCancelled = 2001;

    // Error (4000-4099)
    public const int WorkerLogProcessingError = 4000;
    public const int WorkerLogDeserializationError = 4001;
}
