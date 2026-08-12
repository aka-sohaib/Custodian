using Custodian.Domain.Entities;
using Custodian.Domain.Enums;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Custodian.Infrastructure.Repositories;

public class InvitationRepository : IInvitationRepository
{
    private readonly CustodianDbContext _context;

    public InvitationRepository(CustodianDbContext context) => _context = context;

    public async Task<Invitation?> GetByIdAsync(Guid id)
    {
        return await _context.Invitations.FindAsync(id);
    }

    public async Task<Invitation?> GetByEmailAsync(string email)
    {
        return await _context.Invitations.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<Invitation?> GetByTokenAsync(string token)
    {
        return await _context.Invitations.FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task<IEnumerable<Invitation>> GetByInternalUserRoleAsync(InternalUserRole role)
    {
        return await _context.Invitations.Where(x => x.InternalUserRole == role).ToListAsync();
    }

    public async Task<IEnumerable<Invitation>> GetByVendorUserRoleAsync(VendorUserRole vendorUserRole)
    {
        return await _context.Invitations.Where(x => x.VendorUserRole == vendorUserRole).ToListAsync();
    }

    public async Task<IEnumerable<Invitation>> GetByInvitationTypeAsync(InvitationType invitationType)
    {
        return await _context.Invitations.Where(x => x.InvitationType == invitationType).ToListAsync();
    }

    public async Task<IEnumerable<Invitation>> GetByOrganizationId(Guid organizationId)
    {
        return await _context.Invitations.Where(x => x.OrganizationId == organizationId).ToListAsync();
    }

    public async Task<IEnumerable<Invitation>> GetByInvitedId(Guid invitedById)
    {
        return await _context.Invitations.Where(x => x.InvitedById == invitedById).ToListAsync();
    }

    public async Task AddAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        _context.Invitations.Add(invitation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        _context.Invitations.Update(invitation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
