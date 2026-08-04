using Custodian.Domain.Entities;

namespace Custodian.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
    }
}
