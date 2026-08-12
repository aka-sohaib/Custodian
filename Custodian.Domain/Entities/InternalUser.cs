using Custodian.Domain.Enums;

namespace Custodian.Domain.Entities;

public class InternalUser : User
{
    //---- For EF Core ----
    private InternalUser() { }

    //---- For Factory ----
    private InternalUser(string name, string email, string passwordHash, InternalUserRole role, Guid organizationId) 
        : base(Guid.NewGuid(), name, email, passwordHash, organizationId)
    {
        InternalUserRole = role;
    }

    //---- Factory Method ----
    public static InternalUser CreateInternalUser(string name, string email, string passwordHash, InternalUserRole role, Guid organizationId) 
    {
        if (!Enum.IsDefined(typeof(InternalUserRole), role))
            throw new ArgumentException("Role is not valid.", nameof(role));
        if (Guid.Empty == organizationId)
            throw new ArgumentException("Organization ID is not valid.", nameof(organizationId));

        return new InternalUser(name, email, passwordHash, role, organizationId);
    }

    //---- Update Method ----
    public void Update(string name, string email, string passwordHash, InternalUserRole role)
    {
        base.UpdateBaseUser(name, email, passwordHash);
        if (!Enum.IsDefined(typeof(InternalUserRole), role))
            throw new ArgumentException("Role is not valid.", nameof(role));

        InternalUserRole = role;
        UpdatedAt        = DateTime.UtcNow;
    }

    //---- Properties ----
    public InternalUserRole InternalUserRole { get; private set; }
}
