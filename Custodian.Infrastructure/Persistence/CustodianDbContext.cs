using Microsoft.EntityFrameworkCore;
using Custodian.Domain.Entities;

namespace Custodian.Infrastructure.Persistence
{
    public class CustodianDbContext: DbContext
    {
        public CustodianDbContext(DbContextOptions<CustodianDbContext> options ): base(options) {}
        public DbSet<Vendor> Vendors { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<LineItem> LineItems { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<InvoiceAIAnalysis> Analyses { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustodianDbContext).Assembly);

            //----Hides Soft Deletes (Vendors) ----
            modelBuilder.Entity<Vendor>().HasQueryFilter(v => !v.IsDeleted);
        }
    }
}
