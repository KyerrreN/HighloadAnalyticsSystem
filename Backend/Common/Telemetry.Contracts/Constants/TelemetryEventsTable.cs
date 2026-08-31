namespace Telemetry.Contracts.Constants;

public static class TelemetryEventsTable
{
    public const string TableName = "telemetry_events";

    public const string ProjectId = "ProjectId";
    public const string EventId = "EventId";
    public const string EventName = "EventName";
    public const string Timestamp = "Timestamp";
    public const string ReceivedAt = "ReceivedAt";
    public const string ActorId = "ActorId";
    public const string SessionId = "SessionId";
    public const string Properties = "Properties";
    public const string TraceParent = "TraceParent";

    public static readonly string[] ColumnNames =
    [
        ProjectId,
        EventId,
        EventName,
        Timestamp,
        ReceivedAt,
        ActorId,
        SessionId,
        Properties,
        TraceParent
    ];
}
