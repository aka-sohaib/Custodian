using Custodian.Domain.Entities;
using Custodian.Domain.Enums;
using Custodian.Domain.Interfaces;
using Custodian.Domain.Exceptions;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Custodian.Infrastructure.Repositories;

public class CompanyVendorRepository: ICompanyVendorRepository
{
    private readonly CustodianDbContext _context;
    public CompanyVendorRepository(CustodianDbContext context) => _context = context;
    public async Task<CompanyVendor?> GetByIdAsync(Guid id)
    {
        return await _context.CompanyAndVendorConnections.FindAsync(id);
    }
    public async Task<IEnumerable<CompanyVendor>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.CompanyAndVendorConnections.Where(x=> x.CompanyId == companyId).ToListAsync();
    }
    public async Task<IEnumerable<CompanyVendor>> GetByVendorIdAsync(Guid vendorId)
    {
        return await _context.CompanyAndVendorConnections.Where(x => x.VendorId == vendorId).ToListAsync();
    }
    public async Task<IEnumerable<CompanyVendor>> GetByRequestedUserIdAsync(Guid requestedByUserID)
    {
        return await _context.CompanyAndVendorConnections.Where(x=> x.RequestedById == requestedByUserID).ToListAsync();
    }
    public async Task<IEnumerable<CompanyVendor>> GetByRespondedUserIdAsync(Guid respondedUserID)
    {
        return await _context.CompanyAndVendorConnections.Where(x => x.RespondedById == respondedUserID).ToListAsync();
    }
    public async Task<IEnumerable<CompanyVendor>> GetByCompanyAndVendorIdAsync(Guid companyId, Guid vendorId)
    {
        return await _context.CompanyAndVendorConnections.Where(x => x.CompanyId == companyId && x.VendorId == vendorId).ToListAsync();
    }
    public async Task<IEnumerable<CompanyVendor>> GetByConnectionStatusAsync(ConnectionStatus connectionState)
    {
        return await _context.CompanyAndVendorConnections.Where(x => x.ConnectionStatus == connectionState).ToListAsync();
    }
    public async Task AddAsync(CompanyVendor companyVendor)
    {
        _context.CompanyAndVendorConnections.Add(companyVendor);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(CompanyVendor companyVendor)
    {
        _context.CompanyAndVendorConnections.Update(companyVendor);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var CompanyVendor = await _context.CompanyAndVendorConnections.FindAsync(id);
        
        if(CompanyVendor == null) { throw new NotFound(nameof(CompanyVendor), id); }

        CompanyVendor.Delete();
        await _context.SaveChangesAsync();
    }
}
