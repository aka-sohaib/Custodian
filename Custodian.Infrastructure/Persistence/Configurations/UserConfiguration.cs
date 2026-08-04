using Custodian.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Custodian.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasDiscriminator<string>("UserType")
            .HasValue<InternalUser>("InternalUser")
            .HasValue<VendorUser>("VendorUser");

        builder.HasKey(u => u.Id);

        builder.Property(u=> u.Name)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasMany(u => u.SubmittedInvoices)
            .WithOne(i => i.SubmittedBy)
            .HasForeignKey(i => i.SubmittedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.AuditLogs)
            .WithOne(a => a.PerformedBy)
            .HasForeignKey(a => a.PerformedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
