using Custodian.Domain.Entities;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Custodian.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly CustodianDbContext _context;

    public OrganizationRepository(CustodianDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Organization?> GetByIdAsync(Guid id)
    {
        return await _context.Organizations
            .Include(o => o.Users)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Organization?> GetByEmailAsync(string email)
    {
        return await _context.Organizations
            .FirstOrDefaultAsync(o => o.Email.ToLower() == email.ToLower());
    }

    public async Task AddAsync(Organization organization)
    {
        await _context.Organizations.AddAsync(organization);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Organization organization)
    {
        _context.Organizations.Update(organization);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Organization organization)
    {
        organization.Delete();
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? currentOrganizationId = null)
    {
        return !await _context.Organizations
            .AnyAsync(o => o.Email.ToLower() == email.ToLower() && (!currentOrganizationId.HasValue || o.Id != currentOrganizationId.Value));
    }

    public async Task<bool> IsPhoneUniqueAsync(string phone, Guid? currentOrganizationId = null)
    {
        return !await _context.Organizations
            .AnyAsync(o => o.Phone == phone && (!currentOrganizationId.HasValue || o.Id != currentOrganizationId.Value));
    }
}
