using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemetry.UserManagement.Infrastructure.Database.Entities;

namespace Telemetry.UserManagement.Infrastructure.Database.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(e => e.Email).IsUnique();
    }
}
