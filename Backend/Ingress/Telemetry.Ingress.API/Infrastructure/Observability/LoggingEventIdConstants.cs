namespace Telemetry.Ingress.API.Infrastructure.Observability;

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

    // Channel (X000-X099)
    // Information (2000-2099)
    public const int ChannelLogStarted = 2000;
    public const int ChannelLogCancelled = 2001;

    // Error (4000-4099)
    public const int ChannelLogProcessingError = 4000;
}
