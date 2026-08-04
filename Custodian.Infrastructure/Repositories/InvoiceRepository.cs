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
        public async Task<Invoice?> GetByIdAsync(Guid Id)
        {
            return await _context.Invoices.FindAsync(Id);
        }
        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices.ToListAsync();
        }
        public async Task<IEnumerable<Invoice>> GetByCompanyVendorIdAsync(Guid companyVendorId)
        {
            return await _context.Invoices.Where(i => i.CompanyVendorId == companyVendorId).ToListAsync();
        }
        public async Task<IEnumerable<Invoice>> GetBySubmitterAsync(Guid userId)
        {
            return await _context.Invoices.Where(i => i.SubmittedById == userId).ToListAsync();
        }
        public async Task<IEnumerable<Invoice>> GetByStatusAsync(Status status)
        {
            return await _context.Invoices.Where(i => i.CurrentStatus == status).ToListAsync();
        }
        public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Invoices.Where(i => (i.DueDate > from && i.DueDate < to)).ToListAsync();
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
