namespace Telemetry.Contracts.Constants;

public static class OtelTagConstants
{
    // semantic conventions
    public const string MessagingSystem = "messaging.system";
    public const string MessagingDestinationName = "messaging.destination.name";
    public const string ErrorType = "error.type";
    public const string DatabaseName = "db.system.name";
    public const string CollectionName = "db.collection.name";
    public const string DbBatchSize = "db.operation.batch.size";
    public const string DbOperationName = "db.operation.name";

    // custom
    public const string TelemetryEventName = "telemetry.event_name";
    public const string ProjectApiKeyHash = "project.api_key_hash";
    public const string CacheStatus = "cache.status";
    public const string CacheKey = "cache.key";
}
