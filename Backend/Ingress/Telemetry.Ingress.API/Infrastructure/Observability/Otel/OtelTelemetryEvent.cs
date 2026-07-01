using System.Diagnostics;
using Telemetry.Contracts.Events;

namespace Telemetry.Ingress.API.Infrastructure.Observability.Otel;

public record OtelTelemetryEvent(
    TelemetryEvent Payload,
    ActivityContext TraceContext
);
