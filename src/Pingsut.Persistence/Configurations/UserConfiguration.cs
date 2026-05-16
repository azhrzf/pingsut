using Pingsut.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pingsut.Persistence.Configurations.Extension;

namespace Pingsut.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirebaseId)
            .IsRequired()
            .HasMaxLength(128);

        builder.AddAuditableConfiguration();
    }
}