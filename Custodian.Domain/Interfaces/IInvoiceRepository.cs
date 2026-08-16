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
        Task<(IEnumerable<Invoice> Items, int TotalCount)> GetFilteredInvoicesAsync(
            Guid userId,
            Guid? userOrgId,
            bool isVendor,
            Status? status = null,
            DateTime? fromDueDate = null,
            DateTime? toDueDate = null,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            string? searchTerm = null,
            Guid? organizationConnectionId = null,
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = "CreatedAt",
            bool isDescending = true,
            CancellationToken cancellationToken = default);
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task DeleteAsync(Guid id);
    }
}
