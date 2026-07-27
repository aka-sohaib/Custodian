using Custodian.Domain.Interfaces;

namespace Custodian.Domain.Entities
{
    public class Vendor : BaseEntity, IAuditable
    {
        //---- For EF Core ----
        private Vendor() { }

        //---- For Factory ----
        private Vendor(Guid id, string name, string phone, string contactEmail, int paymentTermDays) : base(id)
        {
            Name            = name;
            Phone           = phone;
            ContactEmail    = contactEmail;
            PaymentTermDays = paymentTermDays;
        }

        //---- Factory Method ----
        public static Vendor Create(string name, string phone, string contactEmail, int paymentTermDays)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Vendor name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(contactEmail))
                throw new ArgumentException("Contact email is required.", nameof(contactEmail));
            if (paymentTermDays < 0)
                throw new ArgumentException("Payment term days cannot be negative.", nameof(paymentTermDays));

            return new Vendor(Guid.NewGuid(), name, phone, contactEmail, paymentTermDays);
        }

        //----Update Method----
        public void Update(string name, string phone, string contactEmail, int paymentTermDays)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Vendor name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(contactEmail))
                throw new ArgumentException("Contact email is required.", nameof(contactEmail));
            if (paymentTermDays < 0)
                throw new ArgumentException("Payment term days cannot be negative.", nameof(paymentTermDays));

            this.Name = name; this.Phone = phone; this.ContactEmail = contactEmail; this.PaymentTermDays = paymentTermDays;
            this.UpdatedAt = DateTime.UtcNow;
        }

        //---- Properties ----
        public string Name { get; private set; } = null!;
        public string Phone { get; private set; } = null!;
        public int PaymentTermDays { get; private set; }
        public string ContactEmail { get; private set; } = null!;

        //---- Navigation Properties ----
        public ICollection<Invoice> Invoices { get; private set; } = new List<Invoice>();
    }
}
