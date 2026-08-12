using Custodian.Domain.Interfaces;

namespace Custodian.Domain.Entities;

public class Organization : BaseEntity, IAuditable
{
    //---- For EF Core ----
    private Organization() { }

    //---- For Factory ----
    private Organization(Guid id, string name, string phone, string email, bool isCompany, bool isVendor) : base(id)
    {
        Name      = name;
        Phone     = phone;
        Email     = email;
        IsCompany = isCompany;
        IsVendor  = isVendor;
    }

    //---- Factory Method ----
    public static Organization Create(string name, string phone, string email, bool isCompany = true, bool isVendor = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required.", nameof(phone));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (!isCompany && !isVendor)
            throw new ArgumentException("Organization must be a company, a vendor, or both.");

        return new Organization(Guid.NewGuid(), name, phone, email, isCompany, isVendor);
    }

    //---- Update Method ----
    public void Update(string name, string phone, string email, bool isCompany, bool isVendor)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required.", nameof(phone));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (!isCompany && !isVendor)
            throw new ArgumentException("Organization must be a company, a vendor, or both.");

        Name      = name;
        Phone     = phone;
        Email     = email;
        IsCompany = isCompany;
        IsVendor  = isVendor;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableCompanyRole()
    {
        if (!IsCompany)
        {
            IsCompany = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void EnableVendorRole()
    {
        if (!IsVendor)
        {
            IsVendor  = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    //---- Properties ----
    public string Name      { get; private set; } = null!;
    public string Phone     { get; private set; } = null!;
    public string Email     { get; private set; } = null!;
    public bool   IsCompany { get; private set; }
    public bool   IsVendor  { get; private set; }

    //---- Navigation Properties ----
    public ICollection<User> Users { get; private set; } = new List<User>();
    
    // Connections where THIS org is the Buyer (Company side)
    public ICollection<OrganizationConnection> VendorConnections { get; private set; } = new List<OrganizationConnection>();
    
    // Connections where THIS org is the Seller (Vendor side)
    public ICollection<OrganizationConnection> ClientConnections { get; private set; } = new List<OrganizationConnection>();
    
    public ICollection<Invitation> Invitations { get; private set; } = new List<Invitation>();
}
