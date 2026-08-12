using Custodian.Domain.Entities;
using Custodian.Domain.Enums;

namespace Custodian.Domain.Interfaces;

public interface IInternalUserRepository
{
    Task<InternalUser?> GetByIdAsync(Guid id);
    Task<IEnumerable<InternalUser>> GetByNameAsync(string name);
    Task<IEnumerable<InternalUser>> GetByOrganizationIdAsync(Guid organizationId);
    Task<IEnumerable<InternalUser>> GetByRoleAsync(InternalUserRole role);
    Task AddAsync(InternalUser user);
    Task UpdateAsync(InternalUser user);
    Task DeleteAsync(Guid internalUserId);
}
