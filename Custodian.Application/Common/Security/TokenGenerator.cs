using System.Security.Cryptography;

namespace Custodian.Application.Common.Security;

public static class TokenGenerator
{
    public static string GenerateInvitationToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }
}
