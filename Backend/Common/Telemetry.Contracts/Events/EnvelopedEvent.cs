using System.Diagnostics;

namespace Telemetry.Contracts.Events;

public record EnvelopedEvent(
    TelemetryEvent Payload,
    ActivityContext TraceContext
);
