using Custodian.Domain.Entities;

namespace Custodian.Domain.Interfaces
{
    public interface IVendorRepository
    {
        Task<Vendor> GetByIdAsync(Guid id);
        Task<IEnumerable<Vendor>> GetAllAsync();
        Task AddAsync(Vendor vendor);
        Task UpdateAsync(Vendor vendor);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeVendorId = null);
        Task DeleteAsync(Guid id);
    }
}
