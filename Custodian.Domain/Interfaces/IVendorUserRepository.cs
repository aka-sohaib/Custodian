using Custodian.Domain.Entities;
using Custodian.Domain.Enums;

namespace Custodian.Domain.Interfaces;

public interface IVendorUserRepository
{
    Task<VendorUser?> GetByIdAsync(Guid id);
    Task<IEnumerable<VendorUser>> GetByNameAsync(string name);
    Task<IEnumerable<VendorUser>> GetByVendorIdAsync(Guid vendorId);
    Task AddAsync(VendorUser user);
    Task UpdateAsync(VendorUser user);
    Task DeleteAsync(Guid VendorUserId);
}
