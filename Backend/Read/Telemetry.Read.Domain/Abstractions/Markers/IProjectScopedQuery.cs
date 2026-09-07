namespace Telemetry.Read.Domain.Abstractions.Markers;

public interface IProjectScopedQuery
{
    Guid ProjectId { get; }
}
