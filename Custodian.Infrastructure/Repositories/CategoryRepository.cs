using Custodian.Domain.Entities;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Custodian.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Custodian.Infrastructure.Repositories
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly CustodianDbContext _context;
        public CategoryRepository(CustodianDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories.FindAsync(id);
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
    }
}
