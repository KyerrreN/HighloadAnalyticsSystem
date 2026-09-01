using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Telemetry.UserManagement.Infrastructure.Database;
using Telemetry.UserManagement.Infrastructure.Database.Options;

namespace Telemetry.UserManagement.API.Factories;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var postgresOptions = builder.Configuration
            .GetSection(PostgresOptions.SectionName)
            .Get<PostgresOptions>();

        if (string.IsNullOrWhiteSpace(postgresOptions?.ConnectionString))
        {
            throw new InvalidOperationException($"ConnectionString is missing in section '{PostgresOptions.SectionName}'");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString, opt =>
        {
            opt.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
        });

        return new AppDbContext(optionsBuilder.Options);
    }
}
