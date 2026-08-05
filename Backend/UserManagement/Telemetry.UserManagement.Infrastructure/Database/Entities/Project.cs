namespace Telemetry.UserManagement.Infrastructure.Database.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public Guid OwnerId { get; set; }

    public User Owner { get; set; } = null!;
    public ICollection<ApiKey> ApiKeys { get; set; } = [];
}
