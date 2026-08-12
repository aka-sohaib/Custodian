using Custodian.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Custodian.Infrastructure.Persistence.Configurations;

public class OrganizationConnectionConfiguration : IEntityTypeConfiguration<OrganizationConnection>
{
    public void Configure(EntityTypeBuilder<OrganizationConnection> builder)
    {
        builder.ToTable("OrganizationConnections");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ConnectionStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.PaymentTermDays)
            .IsRequired();

        // Foreign Key 1: Buyer Organization
        builder.HasOne(c => c.BuyerOrganization)
            .WithMany(o => o.VendorConnections)
            .HasForeignKey(c => c.BuyerOrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Foreign Key 2: Seller Organization
        builder.HasOne(c => c.SellerOrganization)
            .WithMany(o => o.ClientConnections)
            .HasForeignKey(c => c.SellerOrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Foreign Key 3: RequestedBy User
        builder.HasOne(c => c.RequestedBy)
            .WithMany()
            .HasForeignKey(c => c.RequestedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Foreign Key 4: RespondedBy User
        builder.HasOne(c => c.RespondedBy)
            .WithMany()
            .HasForeignKey(c => c.RespondedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Composite Unique Index so an organization can't connect to the same seller organization multiple times
        builder.HasIndex(c => new { c.BuyerOrganizationId, c.SellerOrganizationId })
            .IsUnique();
    }
}
