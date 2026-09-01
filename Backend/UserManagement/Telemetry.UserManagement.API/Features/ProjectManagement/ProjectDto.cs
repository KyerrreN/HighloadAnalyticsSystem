namespace Telemetry.UserManagement.API.Features.ProjectManagement;

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAtUtc);
