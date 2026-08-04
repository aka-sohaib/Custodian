namespace Custodian.Domain.Entities;

public class Company : BaseEntity
{
    //---- For EF Core ----
    private Company() { }

    //---- For Factory ----
    private Company (string name, string phone, string email) : base(Guid.NewGuid())
    {
        Name  = name;
        Phone = phone;
        Email = email;
    }
    
    //---- Factory Method ----
    public static Company CreateCompany (string name, string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name Is Required.", nameof(name));
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone Is Required.", nameof(phone));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email Is Required.", nameof(email));

        return new Company (name, phone, email);
    }

    //---- Update ----
    public void UpdateCompany(string name, string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name Is Required.", nameof(name));
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone Is Required.", nameof(phone));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email Is Required.", nameof(email));

        Name      = name;
        Email     = email;
        Phone     = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    //---- Properties ----
    public string Name  { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    //---- Navigational Properties ----
    public ICollection<InternalUser> InternalUsers { get; private set; } = new List<InternalUser>();
    public ICollection<CompanyVendor> CompanyVendorConnections { get; private set; } = new List<CompanyVendor>();
    public ICollection<Invitation> Invitations { get; private set; } = new List<Invitation>();

}
