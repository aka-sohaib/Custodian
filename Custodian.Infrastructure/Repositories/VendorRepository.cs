using Custodian.Domain.Entities;
using Custodian.Domain.Exceptions;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Custodian.Infrastructure.Repositories
{
    public class VendorRepository: IVendorRepository
    {
        private readonly CustodianDbContext _context;
        public VendorRepository(CustodianDbContext? context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Vendor?> GetByIdAsync(Guid Id)
        {
            return await _context.Vendors.FindAsync(Id);
        }
        public async Task<IEnumerable<Vendor>> GetAllAsync()
        {
            return await _context.Vendors.ToListAsync();
        }
        public async Task AddAsync(Vendor newVendor)
        {
            await _context.Vendors.AddAsync(newVendor);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Vendor updatedVendor)
        {
            _context.Vendors.Update(updatedVendor);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeVendorId = null)
        {
            return !await _context.Vendors
                .Where(v => v.ContactEmail == email && (!excludeVendorId.HasValue || v.Id != excludeVendorId.Value))
                .AnyAsync();
        }
        public async Task DeleteAsync(Guid Id)
        {
            var vendor = await _context.Vendors.FindAsync(Id);
            
            if(vendor == null) { throw new NotFound(nameof(vendor), Id); }
        
            vendor.Delete();
            await _context.SaveChangesAsync();
        }
    }
}