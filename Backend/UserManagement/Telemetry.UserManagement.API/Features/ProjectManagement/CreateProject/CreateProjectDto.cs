namespace Telemetry.UserManagement.API.Features.ProjectManagement.CreateProject;

public record CreateProjectRequestDto(
    string Name,
    string? Description
);

public record CreateProjectResponseDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAtUtc
);