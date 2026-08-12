using Custodian.Domain.Entities;
using Custodian.Domain.Enums;

namespace Custodian.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(Guid id, bool readOnly = false);
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<IEnumerable<Invoice>> GetByOrganizationConnectionIdAsync(Guid organizationConnectionId);
        Task<IEnumerable<Invoice>> GetBySubmitterAsync(Guid userId);
        Task<IEnumerable<Invoice>> GetByStatusAsync(Status status);
        Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task DeleteAsync(Guid id);
    }
}
