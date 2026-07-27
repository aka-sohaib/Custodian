using Custodian.Domain.Entities;
using Custodian.Domain.Enums;
using Custodian.Domain.Exceptions;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Custodian.Infrastructure.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly CustodianDbContext _context;
        public UserRepository(CustodianDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<User> GetByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if(user == null) { throw new NotFound(nameof(User), id); }

            return user;
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null) { throw new NotFound(nameof(User), email); }

            return user;
        }
        public async Task<IEnumerable<User>> GetByRoleAsync(Role role)
        {
            var users = await _context.Users
                                      .Where(x => x.Role == role)
                                      .ToListAsync();

            if (!users.Any()) { return Enumerable.Empty<User>(); }

            return users;
        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid Id)
        {
            var user = await _context.Users.FindAsync(Id);

            if (user == null) { throw new NotFound(nameof(User), Id); }

            user.Delete();
            await _context.SaveChangesAsync();
        }
    }
}
