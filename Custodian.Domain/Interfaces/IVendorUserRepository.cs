using Custodian.Domain.Entities;

namespace Custodian.Domain.Interfaces;

public interface IVendorUserRepository
{
    Task<VendorUser?> GetByIdAsync(Guid id);
    Task<IEnumerable<VendorUser>> GetByNameAsync(string name);
    Task<IEnumerable<VendorUser>> GetByOrganizationIdAsync(Guid organizationId);
    Task AddAsync(VendorUser user);
    Task UpdateAsync(VendorUser user);
    Task DeleteAsync(Guid vendorUserId);
}
