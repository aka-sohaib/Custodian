using Custodian.Domain.Entities;
using Custodian.Domain.Enums;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Custodian.Domain.Exceptions;

namespace Custodian.Infrastructure.Repositories;

public class InternalUserRepository : IInternalUserRepository
{
    private readonly CustodianDbContext _context;
    public InternalUserRepository(CustodianDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<InternalUser?> GetByIdAsync(Guid id)
    {
        return await _context.Users.OfType<InternalUser>().FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<IEnumerable<InternalUser>> GetByNameAsync(string name)
    {
        return await _context.Users.OfType<InternalUser>().Where(x => x.Name == name).ToListAsync();
    }
    public async Task<IEnumerable<InternalUser>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.Users.OfType<InternalUser>().Where(u => u.CompanyId == companyId).ToListAsync();
    }
    public async Task<IEnumerable<InternalUser>> GetByRoleAsync(InternalUserRole role)
    {
        return await _context.Users.OfType<InternalUser>().Where(u => u.InternalUserRole == role).ToListAsync();
    }
    public async Task AddAsync(InternalUser user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(InternalUser user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid InternalUserId)
    {
        var user = await _context.Users.OfType<InternalUser>().FirstOrDefaultAsync(u => u.Id == InternalUserId);

        if (user == null) { throw new NotFound(nameof(InternalUser), InternalUserId); }

        user.Delete();
        await _context.SaveChangesAsync();
    }
}
