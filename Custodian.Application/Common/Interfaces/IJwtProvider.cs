using Custodian.Domain.Enums;

namespace Custodian.Application.Common.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(Guid UserId, string Email, string Role);
    }
}
