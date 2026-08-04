using Custodian.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Custodian.Infrastructure.Persistence.Configurations;

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.ContactEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(v => v.ContactEmail)
            .IsUnique();

        builder.Property(v => v.Phone)
            .HasMaxLength(50);
    }
}
