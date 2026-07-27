using Custodian.Domain.Entities;
using Custodian.Domain.Enums;
using Custodian.Domain.Exceptions;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Custodian.Infrastructure.Repositories
{
    public class InvoiceRepository: IInvoiceRepository
    {
        private readonly CustodianDbContext _context;
        public InvoiceRepository(CustodianDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Invoice> GetByIdAsync(Guid Id)
        {
            var Invoice = await _context.Invoices.FindAsync(Id);
            if(Invoice == null) { throw new NotFound(nameof(Invoice), Id); }
            return Invoice;
        }
        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            var invoices = await _context.Invoices.ToListAsync();
            if (!invoices.Any()) { return Enumerable.Empty<Invoice>(); }
            return invoices;
        }
        public async Task<IEnumerable<Invoice>> GetByVendorAsync(Guid vendorId)
        {
            var invoices = await _context.Invoices
                                                  .Where(i => i.VendorId == vendorId)
                                                  .ToListAsync();
            if (!invoices.Any()) { return Enumerable.Empty<Invoice>(); }
            return invoices;
        }
        public async Task<IEnumerable<Invoice>> GetBySubmitterAsync(Guid userId)
        {
            var invoices = await _context.Invoices
                                                  .Where(i => i.SubmittedById == userId)
                                                  .ToListAsync();
            if (!invoices.Any()) { return Enumerable.Empty<Invoice>(); }
            return invoices;
        }
        public async Task<IEnumerable<Invoice>> GetByStatusAsync(Status status)
        {
            var invoices = await _context.Invoices
                                                  .Where(i => i.CurrentStatus == status)
                                                  .ToListAsync();
            if (!invoices.Any()) { return Enumerable.Empty<Invoice>(); }
            return invoices;
        }
        public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            var invoices = await _context.Invoices
                                                  .Where(i => (i.DueDate > from && i.DueDate < to))
                                                  .ToListAsync();
            if (!invoices.Any()) { return Enumerable.Empty<Invoice>(); }
            return invoices;
        }
        public async Task AddAsync(Invoice invoice)
        {
            await _context.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if(invoice == null) { throw new NotFound(nameof(invoice), id);  }
            
            invoice.Delete();
            await _context.SaveChangesAsync();
        }
    }
}
