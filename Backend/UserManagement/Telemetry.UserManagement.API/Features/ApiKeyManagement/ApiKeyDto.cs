namespace Telemetry.UserManagement.API.Features.ApiKeyManagement;

public sealed record ApiKeyDto(
    Guid Id,
    Guid ProjectId,
    string Name,
    string Prefix,
    bool IsRevoked,
    DateTime CreatedAtUtc,
    DateTime? ExpiresAtUtc,
    DateTime? LastUsedAtUtc);
