namespace Telemetry.UserManagement.Infrastructure.Database.Options;

public class PostgresOptions
{
    public const string SectionName = "PostgreSQL";

    public string ConnectionString { get; set; } = string.Empty;
}
