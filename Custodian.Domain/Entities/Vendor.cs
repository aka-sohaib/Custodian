using Custodian.Domain.Interfaces;

namespace Custodian.Domain.Entities
{
    public class Vendor : BaseEntity, IAuditable
    {
        //---- For EF Core ----
        private Vendor() { }

        //---- For Factory ----
        private Vendor(Guid id, string name, string phone, string contactEmail) : base(id)
        {
            Name            = name;
            Phone           = phone;
            ContactEmail    = contactEmail;
        }

        //---- Factory Method ----
        public static Vendor Create(string name, string phone, string contactEmail)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Vendor name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(contactEmail))
                throw new ArgumentException("Contact email is required.", nameof(contactEmail));

            return new Vendor(Guid.NewGuid(), name, phone, contactEmail);
        }

        //----Update Method----
        public void Update(string name, string phone, string contactEmail)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Vendor name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(contactEmail))
                throw new ArgumentException("Contact email is required.", nameof(contactEmail));

            Name         = name;
            Phone        = phone;
            ContactEmail = contactEmail;
            UpdatedAt    = DateTime.UtcNow;
        }

        //---- Properties ----
        public string Name { get; private set; } = null!;
        public string Phone { get; private set; } = null!;
        public string ContactEmail { get; private set; } = null!;

        //---- Navigation Properties ----
        public ICollection<CompanyVendor> CompanyVendorConnections { get; private set; } = new List<CompanyVendor>();
    }
}
