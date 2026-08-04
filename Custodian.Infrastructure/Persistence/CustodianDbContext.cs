using Microsoft.EntityFrameworkCore;
using Custodian.Domain.Entities;

namespace Custodian.Infrastructure.Persistence
{
    public class CustodianDbContext: DbContext
    {
        public CustodianDbContext(DbContextOptions<CustodianDbContext> options ): base(options) {}
        public DbSet           <Vendor> Vendors       { get; set; } = null!;
        public DbSet          <Company> Companies     { get; set; } = null!;
        public DbSet             <User> Users         { get; set; } = null!;
        public DbSet     <InternalUser> InternalUsers { get; set; } = null!;
        public DbSet       <VendorUser> VendorUsers   { get; set; } = null!;
        public DbSet         <LineItem> LineItems     { get; set; } = null!;
        public DbSet          <Invoice> Invoices      { get; set; } = null!;
        public DbSet<InvoiceAIAnalysis> Analyses      { get; set; } = null!;
        public DbSet         <Category> Categories    { get; set; } = null!;
        public DbSet         <AuditLog> AuditLogs     { get; set; } = null!;
        public DbSet       <Invitation> Invitations   { get; set; } = null!;
        public DbSet    <CompanyVendor> CompanyAndVendorConnections { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustodianDbContext).Assembly);

            //---- Hides Soft Deletes globally & checks parent relationships ----
            modelBuilder.Entity<Company>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Vendor>().HasQueryFilter(v => !v.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<AuditLog>().HasQueryFilter(a => !a.IsDeleted);

            // Company-Vendor Connextions are hidden if deleted OR if company/vendor is deleted
            modelBuilder.Entity<CompanyVendor>().HasQueryFilter(c => !c.IsDeleted && !c.Company.IsDeleted && !c.Vendor.IsDeleted);

            // Invitations are hidden if deleted OR if parent Company/Vendor is deleted
            modelBuilder.Entity<Invitation>().HasQueryFilter(i =>
                !i.IsDeleted && (i.CompanyId == null || !i.Company!.IsDeleted) && (i.VendorId == null || !i.Vendor!.IsDeleted));

            // Invoices are hidden if they are deleted OR if their Vendor is deleted
            modelBuilder.Entity<Invoice>().HasQueryFilter(i => !i.IsDeleted && !i.CompanyAndVendor.IsDeleted);

            // Line items are hidden if they are deleted OR if their Invoice is deleted
            modelBuilder.Entity<LineItem>().HasQueryFilter(l => !l.IsDeleted && !l.Invoice.IsDeleted);

            // Analyses are hidden if they are deleted OR if their Invoice is deleted
            modelBuilder.Entity<InvoiceAIAnalysis>().HasQueryFilter(a => !a.IsDeleted && !a.Invoice.IsDeleted);

            // Users are hidden if they are deleted OR if their subclass parent (Company/Vendor) is deleted
            modelBuilder.Entity<User>().HasQueryFilter(u => 
                !u.IsDeleted &&
                (u is InternalUser == false || !((InternalUser)u).Company.IsDeleted) &&
                (u is VendorUser == false || !((VendorUser)u).Vendor.IsDeleted)
            );
        }
    }
}
