using Custodian.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Custodian.Infrastructure.Persistence.Configurations;

public class InternalUserConfiguration : IEntityTypeConfiguration<InternalUser>
{
    public void Configure(EntityTypeBuilder<InternalUser> builder)
    {
        builder.Property(u => u.CompanyId)
            .IsRequired();

        builder.HasOne(u => u.Company)
            .WithMany(c => c.InternalUsers)
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(u => u.InternalUserRole)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }
}
