using Custodian.Domain.Entities;

namespace Custodian.Domain.Interfaces;

public interface IOrganizationConnectionRepository
{
    Task<OrganizationConnection?> GetByIdAsync(Guid id);
    Task<OrganizationConnection?> GetConnectionAsync(Guid buyerOrganizationId, Guid sellerOrganizationId);
    Task AddAsync(OrganizationConnection connection);
    Task UpdateAsync(OrganizationConnection connection);
    Task DeleteAsync(OrganizationConnection connection);
    Task<IEnumerable<OrganizationConnection>> GetVendorConnectionsForOrganizationAsync(Guid buyerOrganizationId);
    Task<IEnumerable<OrganizationConnection>> GetClientConnectionsForOrganizationAsync(Guid sellerOrganizationId);
}
