namespace Telemetry.UserManagement.API.Features.ApiKeyManagement.CreateApiKey;

public record CreateApiKeyRequest(
    string Name,
    DateTime ExpiresAtUtc);

public record CreateApiKeyResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    string RawKey,
    string Prefix,
    DateTime CreatedAtUtc,
    DateTime? ExpiresAtUtc);
