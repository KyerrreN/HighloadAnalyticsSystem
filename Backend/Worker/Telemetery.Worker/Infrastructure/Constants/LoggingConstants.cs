namespace Telemetry.Worker.Infrastructure.Constants;

public class LoggingConstants
{
    // Information Logs (3000 - 3099)
    public const int PartitionsAssigned = 3000;
    public const int SubscribedToTopic = 3001;
    public const int FlushingBatch = 3002;
    public const int BatchSaved = 3003;
    public const int Stopping = 3004;
    public const int Disconnected = 3005;
    public const int InitializingClickHouse = 3006;
    public const int ClickHouseInitialized = 3007;

    // Error Logs (5000 - 5099)
    public const int DeserializationError = 5000;
    public const int SinkFlushError = 5001;
    public const int ClickHouseInitializationFailed = 5002;
}
