using Custodian.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Custodian.Infrastructure.Persistence.Configurations;

public class VendorUserConfiguration : IEntityTypeConfiguration<VendorUser>
{
    public void Configure(EntityTypeBuilder<VendorUser> builder)
    {
        builder.Property(u => u.VendorUserRole)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }
}
