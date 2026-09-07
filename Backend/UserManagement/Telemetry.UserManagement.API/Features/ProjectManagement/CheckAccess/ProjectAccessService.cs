using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Telemetry.Contracts.Grpc;
using Telemetry.UserManagement.Infrastructure.Database;

namespace Telemetry.UserManagement.API.Features.ProjectManagement.CheckAccess;

public class ProjectAccessService : ProjectAccess.ProjectAccessBase
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ProjectAccessService> _logger;

    public ProjectAccessService(AppDbContext dbContext, ILogger<ProjectAccessService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public override async Task<CheckProjectAccessResponse> CheckProjectAccess(CheckProjectAccessRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userId) || !Guid.TryParse(request.ProjectId, out var projectId))
        {
            _logger.LogWarning("Invalid Guid formats: UserId={UserId}, ProjectId={ProjectId}", request.UserId, request.ProjectId); // todo: high-performance logging
            
            return new CheckProjectAccessResponse
            {
                HasAccess = false
            };
        }

        var result = await _dbContext.Projects
            .AsNoTracking()
            .AnyAsync(p => p.Id == projectId && p.OwnerId == userId);

        return new CheckProjectAccessResponse
        {
            HasAccess = result
        };
    }
}
