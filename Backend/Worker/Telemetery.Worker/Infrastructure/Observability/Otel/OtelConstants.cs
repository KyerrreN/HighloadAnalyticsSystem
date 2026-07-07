namespace Telemetry.Worker.Infrastructure.Observability.Otel;

public static class OtelConstants
{
    public const string TelemetrySinkActivitySourceName = "Telemetry.Worker.ClickHouse";

    public const string KafkaEventsRecievedCounterName = "telemetry.worker.events.consumed";
    public const string PoisonPillsCounterName = "telemetry.worker.poison.pills";
    public const string BatchSizeHistogramName = "telemetry.worker.batch.size";
}
