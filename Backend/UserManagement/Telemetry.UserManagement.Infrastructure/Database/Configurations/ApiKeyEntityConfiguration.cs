using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemetry.UserManagement.Infrastructure.Database.Entities;

namespace Telemetry.UserManagement.Infrastructure.Database.Configurations;

public sealed class ApiKeyEntityConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.HasIndex(e => e.KeyHash).IsUnique();
    }
}
