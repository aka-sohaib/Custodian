using Custodian.Application.Common.Interfaces;
using Isopoh.Cryptography.Argon2;

namespace Custodian.Infrastructure.Security;

public class Argon2PasswordHasher: IPasswordHasher
{
    public string Hash(string password)
    {
        return Argon2.Hash(password);
    }
    public bool Verify(string storedhash, string Password) 
    {
        return Argon2.Verify(storedhash, Password);
    }
}
