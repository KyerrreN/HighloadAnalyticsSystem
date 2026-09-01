namespace Telemetry.UserManagement.Infrastructure.Database.Entities;

public sealed class ApiKey
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public DateTime? LastUsedAtUtc { get; set; }

    public Project Project { get; set; } = null!;
}
