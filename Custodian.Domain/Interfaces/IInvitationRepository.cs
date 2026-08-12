using Custodian.Domain.Entities;
using Custodian.Domain.Enums;

namespace Custodian.Domain.Interfaces;

public interface IInvitationRepository
{
    Task<Invitation?> GetByIdAsync(Guid id);
    Task<Invitation?> GetByEmailAsync(string email);
    Task<Invitation?> GetByTokenAsync(string token);
    Task<IEnumerable<Invitation>> GetByInternalUserRoleAsync(InternalUserRole role);
    Task<IEnumerable<Invitation>> GetByVendorUserRoleAsync(VendorUserRole vendorUserRole);
    Task<IEnumerable<Invitation>> GetByInvitationTypeAsync(InvitationType invitationType);
    Task<IEnumerable<Invitation>> GetByOrganizationId(Guid organizationId);
    Task<IEnumerable<Invitation>> GetByInvitedId(Guid invitedById);
    Task AddAsync(Invitation invitation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Invitation invitation, CancellationToken cancellationToken = default);
}
