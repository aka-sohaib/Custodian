using Custodian.Domain.Entities;
using Custodian.Domain.Enums;

namespace Custodian.Domain.Interfaces;

public interface ICompanyVendorRepository
{
    Task<CompanyVendor?> GetByIdAsync(Guid id);
    Task<IEnumerable<CompanyVendor>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<CompanyVendor>> GetByVendorIdAsync(Guid vendorId);
    Task<IEnumerable<CompanyVendor>> GetByRequestedUserIdAsync(Guid requestedByUserID);
    Task<IEnumerable<CompanyVendor>> GetByRespondedUserIdAsync(Guid respondedUserID);
    Task<IEnumerable<CompanyVendor>> GetByCompanyAndVendorIdAsync(Guid company, Guid vendorId);
    Task<IEnumerable<CompanyVendor>> GetByConnectionStatusAsync(ConnectionStatus connectionState);
    Task AddAsync(CompanyVendor companyVendor);
    Task UpdateAsync(CompanyVendor companyVendor);
    Task DeleteAsync(Guid id);
}
