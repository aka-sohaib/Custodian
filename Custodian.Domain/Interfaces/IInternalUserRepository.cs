
namespace Custodian.Domain.Interfaces;

using Custodian.Domain.Entities;
using Custodian.Domain.Enums;

public interface IInternalUserRepository
{
    Task<InternalUser?> GetByIdAsync(Guid id);
    Task<IEnumerable<InternalUser>> GetByNameAsync(string name);
    Task<IEnumerable<InternalUser>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<InternalUser>> GetByRoleAsync(InternalUserRole role);
    Task AddAsync(InternalUser user);
    Task UpdateAsync(InternalUser user);
    Task DeleteAsync(Guid InternalUserId);
}
