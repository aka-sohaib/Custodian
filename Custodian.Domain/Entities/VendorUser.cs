using Custodian.Domain.Enums;

namespace Custodian.Domain.Entities;

public class VendorUser : User
{
    //---- For EF Core ----
    private VendorUser() { }

    //---- For Factory ----
    private VendorUser(string name, string email, string passwordHash, Guid organizationId, VendorUserRole role) 
        : base(Guid.NewGuid(), name, email, passwordHash, organizationId)
    {
        VendorUserRole = role;
    }

    //---- Factory Method ----
    public static VendorUser CreateVendorUser(string name, string email, string passwordHash, Guid organizationId, VendorUserRole role)
    {
        if (Guid.Empty == organizationId)
            throw new ArgumentException("Organization ID is not valid.", nameof(organizationId));
        if (!Enum.IsDefined(role))
            throw new ArgumentException("Vendor Role is not valid.", nameof(role));

        return new VendorUser(name, email, passwordHash, organizationId, role);
    }

    //---- Update Method ----
    public void Update(string name, string email, string passwordHash, VendorUserRole role)
    {
        if (!Enum.IsDefined(role))
            throw new ArgumentException("Vendor Role is not valid.", nameof(role));
        
        UpdateBaseUser(name, email, passwordHash);

        VendorUserRole = role;
        UpdatedAt      = DateTime.UtcNow;
    }

    //---- Properties ----
    public VendorUserRole VendorUserRole { get; private set; }
}
