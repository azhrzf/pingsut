using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pingsut.Domain.Common;
using Pingsut.Domain.Entities.Common;

namespace Pingsut.Persistence.Configurations.Extension;

public static class AuditableConfiguration
{
    public static void AddAuditableConfiguration<T>(this EntityTypeBuilder<T> builder) where T : AuditableEntity
    {
        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .IsRequired();
    }
}