using Custodian.Domain.Entities;
using Custodian.Domain.Exceptions;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<VendorUser>> GetByOrganizationIdAsync(Guid organizationId)
    {
        return await _context.Users.OfType<VendorUser>().Where(u => u.OrganizationId == organizationId).ToListAsync();
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

    public async Task DeleteAsync(Guid vendorUserId)
    {
        var user = await _context.Users.OfType<VendorUser>().FirstOrDefaultAsync(u => u.Id == vendorUserId);

        if (user == null) { throw new NotFound(nameof(VendorUser), vendorUserId); }

        user.Delete();
        await _context.SaveChangesAsync();
    }
}
