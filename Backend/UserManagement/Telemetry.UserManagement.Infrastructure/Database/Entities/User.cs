namespace Telemetry.UserManagement.Infrastructure.Database.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<Project> Projects { get; set; } = [];
}
