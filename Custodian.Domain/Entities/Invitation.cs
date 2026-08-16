using Custodian.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Custodian.Domain.Entities;

public class Invitation : BaseEntity
{
    //---- For EF Core ----
    private Invitation() { }
    
    //---- For Factory -----
    private Invitation(string email, 
                        string token,
                        InternalUserRole? internalUserRole,
                        VendorUserRole? vendorUserRole, 
                        InvitationType invitationType,
                        Guid organizationId,
                        Guid invitedById) 
                        : base(Guid.NewGuid())
    {
        Email            = email;
        Token            = token;
        InternalUserRole = internalUserRole;
        VendorUserRole   = vendorUserRole;
        InvitationType   = invitationType;
        OrganizationId   = organizationId;
        InvitedById      = invitedById;
        ExpiresAt        = DateTime.UtcNow.AddDays(7);
        AcceptedAt       = null;
    }

    //---- Factory Method: For Internal / Company User Invitation ----
    public static Invitation CreateForInternalUser(string email, string token, InternalUserRole userRole, Guid organizationId, Guid invitedById, InvitationType invitationType = InvitationType.Employee)
    {
        ValidateCommonFields(email, token, invitedById);

        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID is required for internal invitations.", nameof(organizationId));
        if (!Enum.IsDefined(userRole))
            throw new ArgumentException("The internal role is not valid.", nameof(userRole));

        return new Invitation(email, token, internalUserRole: userRole, vendorUserRole: null, invitationType: invitationType,
                              organizationId: organizationId, invitedById: invitedById);
    }

    //---- Factory Method: For Vendor Invitation ----
    public static Invitation CreateForVendorUser(string email, string token, VendorUserRole userRole, Guid organizationId, Guid invitedById)
    {
        ValidateCommonFields(email, token, invitedById);

        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID is required for vendor invitations.", nameof(organizationId));
        if (!Enum.IsDefined(userRole))
            throw new ArgumentException("The vendor role is not valid.", nameof(userRole));

        return new Invitation(email, token, internalUserRole: null, vendorUserRole: userRole, invitationType: InvitationType.Vendor,
                              organizationId: organizationId, invitedById: invitedById);
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

    //---- Accept Invitation ----
    public bool AcceptInvitation()
    {
        if (!IsExpired() && !IsAccepted())
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
    public Guid OrganizationId { get; init; }
    public Guid InvitedById    { get; init; }

    //---- Navigation ----
    public Organization Organization { get; private set; } = null!;
    public User InvitedBy            { get; private set; } = null!;
}
