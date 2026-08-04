using Custodian.Domain.Entities;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Custodian.Domain.Exceptions;

namespace Custodian.Infrastructure.Repositories;

public class VendorUserRepository : IVendorUserRepository
{
    private readonly CustodianDbContext _context;
    public VendorUserRepository(CustodianDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<VendorUser?> GetByIdAsync(Guid id)
    {
        return await _context.Users.OfType<VendorUser>().FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<IEnumerable<VendorUser>> GetByNameAsync(string name)
    {
        return await _context.Users.OfType<VendorUser>().Where(x => x.Name == name).ToListAsync();
    }
    public async Task<IEnumerable<VendorUser>> GetByVendorIdAsync(Guid vendorId)
    {
        return await _context.Users.OfType<VendorUser>().Where(u=>u.VendorId == vendorId).ToListAsync();
    }
    public async Task AddAsync(VendorUser user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(VendorUser user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid VendorUserId)
    {
        var user = await _context.Users.OfType<VendorUser>().FirstOrDefaultAsync(u => u.Id == VendorUserId);

        if (user == null) { throw new NotFound(nameof(VendorUser), VendorUserId); }

        user.Delete();
        await _context.SaveChangesAsync();
    }
}
