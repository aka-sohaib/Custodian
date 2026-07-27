using Custodian.Domain.Enums;

namespace Custodian.Domain.Entities
{
    public class User : BaseEntity
    {
        //---- For EF Core ----
        private User() { }

        //---- For Factory ----
        private User(Guid id, string email, string passwordHash, Role role) : base(id)
        {
            Email        = email;
            PasswordHash = passwordHash;
            Role         = role;
        }

        //---- Factory Method ----
        public static User Create(string email, string passwordHash, Role role)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required.", nameof(passwordHash));

            return new User(Guid.NewGuid(), email, passwordHash, role);
        }

        //---- Update Method ----
        public void Update(string email, string passwordHash, Role role)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required.", nameof(passwordHash));

            this.Email = email; this.PasswordHash = passwordHash; this.Role = role;
        }

        //---- Properties ----
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public Role Role { get; private set; }

        //---- Navigation Properties ----
        public ICollection<Invoice> SubmittedInvoices { get; private set; } = new List<Invoice>();
        public ICollection<AuditLog> AuditLogs { get; private set; } = new List<AuditLog>();
    }
}
