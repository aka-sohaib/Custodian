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
        public async Task<Invoice?> GetByIdAsync(Guid Id, bool readOnly = false)
        {
            var query = _context.Invoices
                                .Include(i => i.LineItems)
                                .AsQueryable();
            if (readOnly)
                query.AsNoTracking();

            return await query.FirstOrDefaultAsync(i=> i.Id == Id);
        }
        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices.ToListAsync();
        }
        public async Task<IEnumerable<Invoice>> GetByOrganizationConnectionIdAsync(Guid organizationConnectionId)
        {
            return await _context.Invoices.Where(i => i.OrganizationConnectionId == organizationConnectionId).ToListAsync();
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

        public async Task<(IEnumerable<Invoice> Items, int TotalCount)> GetFilteredInvoicesAsync(
            Guid userId,
            Guid? userOrgId,
            bool isVendor,
            Status? status = null,
            DateTime? fromDueDate = null,
            DateTime? toDueDate = null,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            string? searchTerm = null,
            Guid? organizationConnectionId = null,
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = "CreatedAt",
            bool isDescending = true,
            CancellationToken cancellationToken = default)
        {
            //---- Start with read-only query ----
            var query = _context.Invoices
                .AsNoTracking()
                .Include(i => i.LineItems)
                .Include(i => i.OrganizationConnection)
                .AsQueryable();

            //---- Enforce tenant security scoping ----
            if (isVendor)
            {
                query = query.Where(i => i.SubmittedById == userId ||
                                        (userOrgId.HasValue && i.OrganizationConnection != null && i.OrganizationConnection.SellerOrganizationId == userOrgId.Value));
            }
            else
            {
                query = query.Where(i => i.SubmittedById == userId ||
                                        (userOrgId.HasValue && i.OrganizationConnection != null && i.OrganizationConnection.BuyerOrganizationId == userOrgId.Value) ||
                                        (userOrgId.HasValue && i.OrganizationConnectionId == null && i.SubmittedBy.OrganizationId == userOrgId.Value));
            }

            //---- Apply optional filters ----
            if (status.HasValue)
            {
                query = query.Where(i => i.CurrentStatus == status.Value);
            }

            if (fromDueDate.HasValue)
            {
                query = query.Where(i => i.DueDate >= fromDueDate.Value);
            }

            if (toDueDate.HasValue)
            {
                query = query.Where(i => i.DueDate <= toDueDate.Value);
            }

            if (minAmount.HasValue)
            {
                query = query.Where(i => i.TotalAmount >= minAmount.Value);
            }

            if (maxAmount.HasValue)
            {
                query = query.Where(i => i.TotalAmount <= maxAmount.Value);
            }

            if (organizationConnectionId.HasValue)
            {
                query = query.Where(i => i.OrganizationConnectionId == organizationConnectionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(i => i.InvoiceNumber.ToLower().Contains(term) ||
                                        (i.UnregisteredVendorName != null && i.UnregisteredVendorName.ToLower().Contains(term)));
            }

            //---- Count total matching items before pagination ----
            int totalCount = await query.CountAsync(cancellationToken);

            //---- Apply dynamic sorting ----
            query = sortBy switch
            {
                "TotalAmount"   => isDescending ? query.OrderByDescending(i => i.TotalAmount) : query.OrderBy(i => i.TotalAmount),
                "DueDate"       => isDescending ? query.OrderByDescending(i => i.DueDate) : query.OrderBy(i => i.DueDate),
                "InvoiceNumber" => isDescending ? query.OrderByDescending(i => i.InvoiceNumber) : query.OrderBy(i => i.InvoiceNumber),
                "Status"        => isDescending ? query.OrderByDescending(i => i.CurrentStatus) : query.OrderBy(i => i.CurrentStatus),
                _               => isDescending ? query.OrderByDescending(i => i.CreatedAt) : query.OrderBy(i => i.CreatedAt)
            };

            //---- Apply pagination ----
            int validPageSize = pageSize > 0 ? pageSize : 10;
            int validPageNumber = pageNumber > 0 ? pageNumber : 1;

            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
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
