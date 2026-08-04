using System.ComponentModel.DataAnnotations;

namespace Custodian.Domain.Entities;

public abstract class User : BaseEntity
{
    protected User () { }
    protected User (Guid id, string name, string email, string password) : base(id)
    {
        ValidateAndSet(name, email, password);
    }
    //---- Update Base Method ----
    protected void UpdateBaseUser(string name, string email, string passwordHash)
    {
        ValidateAndSet (name, email, passwordHash);
    }

    //---- Validation For Base Properties ----
    private void ValidateAndSet(string name, string email, string passwordHash)
    {
        var emailValidator = new EmailAddressAttribute();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (!emailValidator.IsValid(email))
            throw new ArgumentException("Invalid email format.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        Name         = name;
        Email        = email;
        PasswordHash = passwordHash;
        UpdatedAt    = DateTime.UtcNow;
    }

    //---- Properties ----
    public string Name         { get; protected set; } = null!;
    public string Email        { get; protected set; } = null!;
    public string PasswordHash { get; protected set; } = null!;

    //---- Navigation Properties ----
    public ICollection<Invoice> SubmittedInvoices { get; private set; } = new List<Invoice>();
    public ICollection<AuditLog> AuditLogs { get; private set; } = new List<AuditLog>();
}