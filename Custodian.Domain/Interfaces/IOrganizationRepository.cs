using Custodian.Domain.Entities;

namespace Custodian.Domain.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id);
    Task<Organization?> GetByEmailAsync(string email);
    Task AddAsync(Organization organization);
    Task UpdateAsync(Organization organization);
    Task DeleteAsync(Organization organization);
    Task<bool> IsEmailUniqueAsync(string email, Guid? currentOrganizationId = null);
    Task<bool> IsPhoneUniqueAsync(string phone, Guid? currentOrganizationId = null);
}
