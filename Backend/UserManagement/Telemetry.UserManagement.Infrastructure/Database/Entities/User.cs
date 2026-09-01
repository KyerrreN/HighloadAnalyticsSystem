namespace Telemetry.UserManagement.Infrastructure.Database.Entities;

public sealed class User
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<Project> Projects { get; set; } = [];
}
