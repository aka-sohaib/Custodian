using Custodian.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Custodian.Domain.Entities;

public class Invitation : BaseEntity
{
    //---- For EF Core ----
    private Invitation () { }
    
    //---- For Factory -----
    private Invitation (string email, 
                        string token,
                        InternalUserRole? internalUserRole,
                        VendorUserRole? vendorUserRole, 
                        InvitationType invitationType,
                        Guid? companyId,
                        Guid? vendorId,
                        Guid invitedById) 
                        : base(Guid.NewGuid())
    {
        Email            = email;
        Token            = token;
        InternalUserRole = internalUserRole;
        VendorUserRole   = vendorUserRole;
        InvitationType   = invitationType;
        CompanyId        = companyId;
        VendorId         = vendorId;
        InvitedById      = invitedById;
        ExpiresAt        = DateTime.UtcNow.AddDays(7);
        AcceptedAt       = null;
    }

    //---- Factory Method: For Employee Invitation ----
    public static Invitation CreateForInternalUser (string email, string token, InternalUserRole userRole, Guid companyId, Guid invitedById)
    {
        ValidateCommonFields(email, token, invitedById);

        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required for internal invitations.", nameof(companyId));
        if (!Enum.IsDefined(userRole))
            throw new ArgumentException("The internal role is not valid.", nameof(userRole));

        return new Invitation(email, token, internalUserRole: userRole, vendorUserRole: null, invitationType: InvitationType.Employee,
                              companyId: companyId, vendorId: null, invitedById: invitedById);
    }

    //---- Factory Method: For Vendor Invitation ----
    public static Invitation CreateForVendorUser(string email, string token, VendorUserRole userRole, Guid vendorId, Guid invitedById)
    {
        ValidateCommonFields(email, token, invitedById);

        if (vendorId == Guid.Empty)
            throw new ArgumentException("vendor ID is required for vendor invitations.", nameof(vendorId));
        if (!Enum.IsDefined(userRole))
            throw new ArgumentException("The vendor role is not valid.", nameof(userRole));

        return new Invitation(email, token, internalUserRole: null, vendorUserRole: userRole, invitationType: InvitationType.Vendor,
                              companyId: null, vendorId: vendorId, invitedById: invitedById);
    }
    //---- Validation Helper ----
    private static void ValidateCommonFields(string email, string token, Guid invitedById)
    {
        var emailValidator = new EmailAddressAttribute();
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Invitation Email cannot be empty.", nameof(email));
        if (!emailValidator.IsValid(email))
            throw new ArgumentException($"The email '{email}' is not a valid email address.", nameof(email));
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token is uninitialized.", nameof(token));
        if (invitedById == Guid.Empty)
            throw new ArgumentException("InvitedById cannot be empty.", nameof(invitedById));
    }

    //---- Common Queries ----
    public bool IsExpired()  => DateTime.UtcNow > ExpiresAt;
    public bool IsAccepted() => AcceptedAt != null;

    //---- Accpet Inivitation ----
    public bool AcceptInvitation()
    {
        if(!IsExpired() && !IsAccepted())
        {
            AcceptedAt = DateTime.UtcNow;
            UpdatedAt  = DateTime.UtcNow;
            return true;
        }
        return false;
    }

    //---- Properties ----
    public string            Email            { get; init; }        = string.Empty;
    public string            Token            { get; private set; } = string.Empty;
    public InternalUserRole? InternalUserRole { get; init; }
    public VendorUserRole?   VendorUserRole   { get; init; }
    public InvitationType    InvitationType   { get; init; }
    public DateTime          ExpiresAt        { get; init; }
    public DateTime?         AcceptedAt       { get; private set; }

    //---- Foreign Keys ----
    public Guid? CompanyId   { get; init; }
    public Guid? VendorId    { get; init; }
    public Guid InvitedById  { get; init; }

    //---- Navigation ----
    public Company? Company       { get; private set; }
    public Vendor?  Vendor        { get; private set; }
    public User InvitedBy { get; private set; } = null!;
}
