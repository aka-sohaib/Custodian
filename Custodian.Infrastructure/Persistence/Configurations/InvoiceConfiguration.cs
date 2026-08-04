using Custodian.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Custodian.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(i => new { i.CompanyVendorId, i.InvoiceNumber })
            .IsUnique();

        builder.Property(i=> i.CompanyVendorId)
            .IsRequired();

        builder.Property(i => i.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(i => i.CurrencyCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(i => i.CurrentStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(i => i.DueDate)
            .IsRequired();

        builder.HasMany(i => i.LineItems)
            .WithOne(l => l.Invoice)
            .HasForeignKey(l => l.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.AIAnalysis)
            .WithOne(a => a.Invoice)
            .HasForeignKey<InvoiceAIAnalysis>(a => a.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.CompanyAndVendor)
            .WithMany()
            .HasForeignKey(i => i.CompanyVendorId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
