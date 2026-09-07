using Microsoft.AspNetCore.Http;
using System.Security.Authentication;
using Telemetry.Contracts.Utils;
using Telemetry.Read.Domain.Abstractions.Markers;
using Telemetry.Read.Domain.Services;

namespace Telemetry.Read.Domain.Abstractions.Decorator;

public sealed class ProjectAuthorizationDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _inner;
    private readonly IProjectAccessService _service;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProjectAuthorizationDecorator(
        IQueryHandler<TQuery, TResponse> inner, 
        IProjectAccessService service, 
        IHttpContextAccessor httpContextAccessor)
    {
        _inner = inner;
        _service = service;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        if (query is not IProjectScopedQuery projectQuery)
        {
            return await _inner.HandleAsync(query, cancellationToken);
        }

        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is unavailable");

        var userIdClaim = AuthUtils.GetUserIdFromClaimsPrincipal(httpContext.User);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new AuthenticationException("User claim is missing or invalid");
        }

        bool hasAccess = await _service.HasAccessAsync(userId, projectQuery.ProjectId, cancellationToken);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException();
        }

        return await _inner.HandleAsync(query, cancellationToken);
    }
}
