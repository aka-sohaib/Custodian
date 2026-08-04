using Custodian.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Custodian.Infrastructure.Persistence.Configurations;

public class CompanyVendorConfiguration: IEntityTypeConfiguration<CompanyVendor>
{
    public void Configure(EntityTypeBuilder<CompanyVendor> builder)
    {

        builder.HasKey(u => u.Id);

        builder.Property(c=> c.CompanyId).IsRequired();
        builder.Property(c=> c.VendorId).IsRequired();
        builder.Property(c=> c.RequestedById).IsRequired();
        builder.Property(c => c.PaymentTermDays).IsRequired();

        builder.Property(c => c.ConnectionStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        
        builder.ToTable(t=> t.HasCheckConstraint("CK_CompanyVendor_PaymentTermDays_GreaterThanOne", "\"PaymentTermDays\" >= 1"));

        builder.HasIndex(c => new { c.CompanyId, c.VendorId }).IsUnique();

        builder.HasOne(c=> c.Vendor)
            .WithMany(v=> v.CompanyVendorConnections)
            .HasForeignKey(c => c.VendorId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Company)
            .WithMany(comp => comp.CompanyVendorConnections)
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.InternalUser)
            .WithMany()
            .HasForeignKey(c => c.RequestedById)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.VendorUser)
            .WithMany()
            .HasForeignKey(c => c.RespondedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
