using Custodian.Domain.Entities;
using Custodian.Domain.Interfaces;
using Custodian.Domain.Exceptions;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Custodian.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly CustodianDbContext _context;
    public CompanyRepository(CustodianDbContext context) => _context = context;
    public async Task<Company?> GetById(Guid id)
    {
        return await _context.Companies.FindAsync(id);
    }
    public async Task<Company?> GetByEmailAsync(string email)
    {
        return await _context.Companies.FirstOrDefaultAsync(x => x.Email == email);
    }
    public async Task<Company?> GetByPhoneAsync(string phone)
    {
        return await _context.Companies.FirstOrDefaultAsync(c=> c.Phone == phone);
    }
    public async Task<IEnumerable<Company>> GetByNameAsync(string name)
    {
        return await _context.Companies.Where(x => x.Name == name).ToListAsync();
    }
    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeCompanyId = null)
    {
        return !await _context.Companies
                .Where(c=> c.Email == email && (!excludeCompanyId.HasValue || c.Id != excludeCompanyId.Value))
                .AnyAsync();
    }
    public async Task AddAsync(Company company)
    {
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Company company)
    {
        _context.Companies.Update(company);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var company = await _context.Companies.FindAsync(id);

        if(company == null) { throw new NotFound(nameof(company), id); }

        company.Delete();
        await _context.SaveChangesAsync();
    }
}
