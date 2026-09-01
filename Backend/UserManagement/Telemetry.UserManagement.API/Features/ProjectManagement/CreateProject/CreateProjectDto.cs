namespace Telemetry.UserManagement.API.Features.ProjectManagement.CreateProject;

public sealed record CreateProjectRequestDto(
    string Name,
    string? Description
);

public sealed record CreateProjectResponseDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAtUtc
);