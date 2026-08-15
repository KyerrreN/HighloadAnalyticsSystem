namespace Telemetry.UserManagement.API.Features.ProjectManagement;

public record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAtUtc);
