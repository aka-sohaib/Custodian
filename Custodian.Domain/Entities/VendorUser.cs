using Custodian.Domain.Enums;

namespace Custodian.Domain.Entities;

public class VendorUser : User
{
    //---- For EF Core ----
    private VendorUser() { }

    //---- For Factory ----
    private VendorUser(string name, string email, string passwordHash, Guid vendorId, VendorUserRole role) : base(Guid.NewGuid(), name, email, passwordHash)
    {
        VendorId       = vendorId;
        VendorUserRole = role;
    }

    //---- Factory Method ----
    public static VendorUser CreateVendorUser(string name, string email, string passwordHash, Guid vendorId, VendorUserRole role)
    {
        if (Guid.Empty == vendorId)
            throw new ArgumentException("Vendor ID is not valid.", nameof(vendorId));
        if(!Enum.IsDefined(role))
            throw new ArgumentException("Vendor Role is not valid.", nameof(role));

        return new VendorUser(name, email, passwordHash, vendorId, role);
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
    public Guid VendorId           { get; private set; }
    public VendorUserRole VendorUserRole { get; private set; }

    //---- Navigational Properties ----
    public Vendor Vendor { get; private set; } = null!;
}
