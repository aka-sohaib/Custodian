using Custodian.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Custodian.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration: IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x=> x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.Email)
            .IsUnique();
    }
}
