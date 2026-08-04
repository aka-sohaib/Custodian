using Custodian.Domain.Entities;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Custodian.Infrastructure.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly CustodianDbContext _context;
        public UserRepository(CustodianDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null)
        {
            return !await _context.Users
                .Where(v => v.Email == email && (!excludeUserId.HasValue || v.Id != excludeUserId.Value))
                .AnyAsync();
        }
    }
}
