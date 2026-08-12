using Custodian.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Custodian.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(o => o.Email)
            .IsUnique();

        builder.Property(o => o.Phone)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(o => o.Phone)
            .IsUnique();

        builder.Property(o => o.IsCompany)
            .IsRequired();

        builder.Property(o => o.IsVendor)
            .IsRequired();

        builder.HasMany(o => o.Users)
            .WithOne(u => u.Organization)
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Invitations)
            .WithOne(i => i.Organization)
            .HasForeignKey(i => i.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
