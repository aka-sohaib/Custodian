using Custodian.Domain.Entities;

namespace Custodian.Domain.Interfaces;

public interface ICompanyRepository
{
    Task<Company?>              GetById(Guid id);
    Task<Company?>              GetByEmailAsync(string email);
    Task<Company?>              GetByPhoneAsync(string phone);
    Task<IEnumerable<Company>> GetByNameAsync(string name);
    Task<bool>                 IsEmailUniqueAsync(string email, Guid? excludeCompanyId);
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task DeleteAsync(Guid id);
}
