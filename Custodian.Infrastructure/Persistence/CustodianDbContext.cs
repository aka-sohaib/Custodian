using Microsoft.EntityFrameworkCore;
using Custodian.Domain.Entities;

namespace Custodian.Infrastructure.Persistence
{
    public class CustodianDbContext : DbContext
    {
        public CustodianDbContext(DbContextOptions<CustodianDbContext> options) : base(options) { }

        public DbSet<Organization> Organizations { get; set; } = null!;
        public DbSet<OrganizationConnection> OrganizationConnections { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<InternalUser> InternalUsers { get; set; } = null!;
        public DbSet<VendorUser> VendorUsers { get; set; } = null!;
        public DbSet<LineItem> LineItems { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<InvoiceAIAnalysis> Analyses { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<Invitation> Invitations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustodianDbContext).Assembly);

            //---- Hides Soft Deletes globally & checks parent relationships ----
            modelBuilder.Entity<Organization>().HasQueryFilter(o => !o.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<AuditLog>().HasQueryFilter(a => !a.IsDeleted);

            // Organization Connections are hidden if deleted OR if either organization is deleted
            modelBuilder.Entity<OrganizationConnection>().HasQueryFilter(c =>
                !c.IsDeleted && !c.BuyerOrganization.IsDeleted && !c.SellerOrganization.IsDeleted);

            // Invitations are hidden if deleted OR if parent Organization is deleted
            modelBuilder.Entity<Invitation>().HasQueryFilter(i =>
                !i.IsDeleted && !i.Organization.IsDeleted);

            // Invoices are hidden if they are deleted OR if their Organization Connection is deleted
            modelBuilder.Entity<Invoice>().HasQueryFilter(i =>
                !i.IsDeleted && !i.OrganizationConnection.IsDeleted);

            // Line items are hidden if they are deleted OR if their Invoice is deleted
            modelBuilder.Entity<LineItem>().HasQueryFilter(l =>
                !l.IsDeleted && !l.Invoice.IsDeleted);

            // Analyses are hidden if they are deleted OR if their Invoice is deleted
            modelBuilder.Entity<InvoiceAIAnalysis>().HasQueryFilter(a =>
                !a.IsDeleted && !a.Invoice.IsDeleted);

            // Users are hidden if they are deleted OR if their Organization is deleted
            modelBuilder.Entity<User>().HasQueryFilter(u =>
                !u.IsDeleted &&
                (u is InternalUser == false || !((InternalUser)u).Organization.IsDeleted) &&
                (u is VendorUser == false || !((VendorUser)u).Organization.IsDeleted)
            );
        }
    }
}
