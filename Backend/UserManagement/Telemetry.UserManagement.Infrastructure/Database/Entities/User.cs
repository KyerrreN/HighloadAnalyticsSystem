namespace Telemetry.UserManagement.Infrastructure.Database.Entities;

public class User
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<Project> Projects { get; set; } = [];
}
