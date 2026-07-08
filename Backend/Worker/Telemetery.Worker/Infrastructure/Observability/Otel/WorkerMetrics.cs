using System.Diagnostics.Metrics;

namespace Telemetry.Worker.Infrastructure.Observability.Otel;

public class WorkerMetrics : IDisposable
{
    public const string MeterName = "Telemetry.Worker";

    private readonly Meter _meter;

    private readonly Counter<long> _kafkaEventsConsumerCounter;
    private readonly Counter<long> _poisonPillsCounter;
    private readonly Histogram<int> _batchSizeHistogram;

    public WorkerMetrics()
    {
        _meter = new Meter(MeterName);

        _kafkaEventsConsumerCounter = _meter.CreateCounter<long>(
            name: OtelConstants.KafkaEventsRecievedCounterName,
            description: "Total number of valid telemetry events consumed from Kafka");

        _poisonPillsCounter = _meter.CreateCounter<long>(
            name: OtelConstants.PoisonPillsCounterName,
            description: "Total number of malformed messages (poison pills) skipped");

        _batchSizeHistogram = _meter.CreateHistogram<int>(
            name: OtelConstants.BatchSizeHistogramName,
            unit: "events",
            description: "Distribution of batch sizes sent to ClickHouse");
    }

    public void RecordEventsConsumed(int count)
    {
        _kafkaEventsConsumerCounter.Add(count);
    }

    public void RecordPoisonPill()
    {
        _poisonPillsCounter.Add(1);
    }

    public void RecordBatchSize(int size)
    {
        _batchSizeHistogram.Record(size);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _meter.Dispose();
    }
}
