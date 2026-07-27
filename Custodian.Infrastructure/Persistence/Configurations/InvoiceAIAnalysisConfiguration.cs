using Custodian.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Custodian.Infrastructure.Persistence.Configurations;

public class InvoiceAIAnalysisConfiguration : IEntityTypeConfiguration<InvoiceAIAnalysis>
{
    public void Configure(EntityTypeBuilder<InvoiceAIAnalysis> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.ConfidenceScore)
            .HasPrecision(5, 4);

        builder.Property(a => a.AnomalyReason)
            .HasMaxLength(1000);
    }
}
