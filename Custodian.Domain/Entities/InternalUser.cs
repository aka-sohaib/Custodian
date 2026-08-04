using Custodian.Domain.Enums;

namespace Custodian.Domain.Entities;

public class InternalUser : User
{
    //---- For EF Core ----
    private InternalUser() { }

    //---- For Factory ----
    private InternalUser(string name, string email, string passwordHash, InternalUserRole role, Guid companyId) : base( Guid.NewGuid() ,name, email, passwordHash)
    {
        InternalUserRole = role;
        CompanyId        = companyId;
    }

    //---- Factory Method ----
    public static InternalUser CreateInternalUser(string name,string email, string passwordHash, InternalUserRole role, Guid companyId) 
    {
        if (!Enum.IsDefined(typeof(InternalUserRole), role))
            throw new ArgumentException("Role is not valid.", nameof(role));
        if (Guid.Empty == companyId)
            throw new ArgumentException("Company ID is not valid.", nameof(companyId));

        return new InternalUser(name, email, passwordHash, role, companyId);
    }

    //---- Update Method ----
    public void Update(string name, string email, string passwordHash ,InternalUserRole role)
    {
        base.UpdateBaseUser(name, email, passwordHash);
        if (!Enum.IsDefined(typeof(InternalUserRole), role))
            throw new ArgumentException("Role is not valid.", nameof(role));

        InternalUserRole = role;
        UpdatedAt        = DateTime.UtcNow;
    }

    //---- Properties ----
    public InternalUserRole InternalUserRole { get; private set; }
    public Guid CompanyId                    { get; private set; }

    //---- Navigational Properties ----
    public Company Company { get; private set; } = null!;
}
