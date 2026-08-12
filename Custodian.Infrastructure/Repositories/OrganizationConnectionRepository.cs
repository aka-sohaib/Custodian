using Custodian.Domain.Entities;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Custodian.Infrastructure.Repositories;

public class OrganizationConnectionRepository : IOrganizationConnectionRepository
{
    private readonly CustodianDbContext _context;

    public OrganizationConnectionRepository(CustodianDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<OrganizationConnection?> GetByIdAsync(Guid id)
    {
        return await _context.OrganizationConnections
            .Include(c => c.BuyerOrganization)
            .Include(c => c.SellerOrganization)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<OrganizationConnection?> GetConnectionAsync(Guid buyerOrganizationId, Guid sellerOrganizationId)
    {
        return await _context.OrganizationConnections
            .FirstOrDefaultAsync(c => c.BuyerOrganizationId == buyerOrganizationId && c.SellerOrganizationId == sellerOrganizationId);
    }

    public async Task AddAsync(OrganizationConnection connection)
    {
        await _context.OrganizationConnections.AddAsync(connection);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrganizationConnection connection)
    {
        _context.OrganizationConnections.Update(connection);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(OrganizationConnection connection)
    {
        connection.Delete();
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrganizationConnection>> GetVendorConnectionsForOrganizationAsync(Guid buyerOrganizationId)
    {
        return await _context.OrganizationConnections
            .Include(c => c.SellerOrganization)
            .Where(c => c.BuyerOrganizationId == buyerOrganizationId)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrganizationConnection>> GetClientConnectionsForOrganizationAsync(Guid sellerOrganizationId)
    {
        return await _context.OrganizationConnections
            .Include(c => c.BuyerOrganization)
            .Where(c => c.SellerOrganizationId == sellerOrganizationId)
            .ToListAsync();
    }
}
