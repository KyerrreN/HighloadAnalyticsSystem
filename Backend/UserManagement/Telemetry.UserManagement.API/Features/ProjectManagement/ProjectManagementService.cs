using Microsoft.EntityFrameworkCore;
using Telemetry.UserManagement.API.Features.ProjectManagement.CreateProject;
using Telemetry.UserManagement.Infrastructure.Database;
using Telemetry.UserManagement.Infrastructure.Database.Entities;
using Telemetry.UserManagement.Infrastructure.Result;

namespace Telemetry.UserManagement.API.Features.ProjectManagement;

public class ProjectManagementService : IProjectManagementService
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ProjectManagementService> _logger;

    public ProjectManagementService(AppDbContext dbContext, ILogger<ProjectManagementService> logger, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    public async Task<Result<CreateProjectResponseDto>> CreateProjectAsync(Guid ownerId, CreateProjectRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result.Failed<CreateProjectResponseDto>(ProjectErrors.EmptyName);
        }

        var trimmedName = request.Name.Trim();

        var exists = await _dbContext.Projects.AnyAsync(p => p.OwnerId == ownerId && p.Name == trimmedName, ct);

        if (exists)
        {
            return Result.Failed<CreateProjectResponseDto>(ProjectErrors.AlreadyExists);
        }

        var project = new Project
        {
            Name = trimmedName,
            Description = request.Description,
            OwnerId = ownerId,
            CreatedAtUtc = _timeProvider.GetUtcNow().UtcDateTime
        };

        try
        {
            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync(ct);

            var response = new CreateProjectResponseDto(
                project.Id,
                project.Name,
                project.Description,
                project.CreatedAtUtc);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create project {ProjectName} for user {UserId}", project.Name, ownerId); // todo: high-performance logging

            return Result.Failed<CreateProjectResponseDto>(ProjectErrors.CreationFailed);
        }
    }

    public async Task<Result<IReadOnlyList<ProjectDto>>> GetAllProjectsAsync(Guid ownerId, CancellationToken ct = default)
    {
        try
        {
            var projects = await _dbContext.Projects
                .AsNoTracking()
                .Where(p => p.OwnerId == ownerId && p.IsDeleted == false)
                .OrderByDescending(p => p.CreatedAtUtc)
                .Select(p => new ProjectDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.CreatedAtUtc))
                .ToListAsync(ct);

            return projects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch projects for user {UserId}", ownerId);
            return Result.Failed<IReadOnlyList<ProjectDto>>(ProjectErrors.FetchFailed);
        }
    }
}

public interface IProjectManagementService
{
    Task<Result<CreateProjectResponseDto>> CreateProjectAsync(
        Guid ownerId,
        CreateProjectRequestDto request,
        CancellationToken ct = default);

    Task<Result<IReadOnlyList<ProjectDto>>> GetAllProjectsAsync(Guid ownerId, CancellationToken ct = default);
}
