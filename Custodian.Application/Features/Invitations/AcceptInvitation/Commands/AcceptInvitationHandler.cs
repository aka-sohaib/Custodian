using Custodian.Application.Common.Exceptions;
using Custodian.Application.Common.Interfaces;
using Custodian.Domain.Entities;
using Custodian.Domain.Enums;
using Custodian.Domain.Interfaces;
using MediatR;

namespace Custodian.Application.Features.Invitations.AcceptInvitation.Commands;

public class AcceptInvitationHandler: IRequestHandler<AccepInvitationCommand, Guid>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IVendorUserRepository _vendorRepository;
    private readonly IInternalUserRepository _internalUserRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOrganizationConnectionRepository _connectionRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AcceptInvitationHandler(
        IInvitationRepository invitationRepository,
        IVendorUserRepository vendorRepository,
        IInternalUserRepository internalUserRepository,
        IOrganizationRepository organizationRepository,
        IOrganizationConnectionRepository connectionRepository,
        IPasswordHasher passwordHasher)
    {
        _invitationRepository = invitationRepository;
        _vendorRepository = vendorRepository;
        _internalUserRepository = internalUserRepository;
        _organizationRepository = organizationRepository;
        _connectionRepository = connectionRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(AccepInvitationCommand command, CancellationToken cancellationToken)
    {
        //---- Token Validation ----
        var invitation = await _invitationRepository.GetByTokenAsync(command.Token) ?? 
            throw new NotFoundException("The invitation link is invalid or corrupted.");

        if (invitation.IsAccepted()) throw new ConflictException("This invitation has already been used.");
        if (invitation.IsExpired()) throw new ConflictException("This invitation has expired.");

        //---- Hash Password ----
        var hashedPassword = _passwordHasher.Hash(command.Password);

        //---- Check User Type & create entity ----
        Guid newUserId = Guid.Empty;

        if (invitation.InvitationType == InvitationType.Vendor && invitation.VendorUserRole is VendorUserRole vendorUserRole)
        {
            if (string.IsNullOrWhiteSpace(command.OrganizationName) || string.IsNullOrWhiteSpace(command.OrganizationPhone))
            {
                throw new ArgumentException("Organization name and phone number are required when registering a vendor.");
            }

            //---- 1. Create Vendor Organization ----
            var orgEmail = string.IsNullOrWhiteSpace(command.OrganizationEmail) ? invitation.Email : command.OrganizationEmail;
            var vendorOrg = Organization.Create(
                name: command.OrganizationName,
                phone: command.OrganizationPhone,
                email: orgEmail,
                isCompany: false,
                isVendor: true
            );
            await _organizationRepository.AddAsync(vendorOrg);

            //---- 2. Create Vendor User belonging to new Vendor Organization ----
            var vendorUser = VendorUser.CreateVendorUser(command.Name, invitation.Email, hashedPassword, vendorOrg.Id, vendorUserRole);
            await _vendorRepository.AddAsync(vendorUser);
            newUserId = vendorUser.Id;

            //---- 3. Create active B2B OrganizationConnection between Buyer & Vendor ----
            var connection = OrganizationConnection.CreateConnection(
                buyerOrganizationId: invitation.OrganizationId,
                sellerOrganizationId: vendorOrg.Id,
                requestedById: invitation.InvitedById,
                paymentTermDays: 30
            );
            connection.AcceptConnection(vendorUser.Id);
            await _connectionRepository.AddAsync(connection);
        }
        else if (invitation.InvitationType == InvitationType.Employee)
        {
            if (invitation.InternalUserRole is InternalUserRole internalUserRole)
            {
                var internalUser = InternalUser.CreateInternalUser(command.Name, invitation.Email, hashedPassword, internalUserRole, invitation.OrganizationId);
                await _internalUserRepository.AddAsync(internalUser);
                newUserId = internalUser.Id;
            }
            else if (invitation.VendorUserRole is VendorUserRole employeeVendorRole)
            {
                var vendorUser = VendorUser.CreateVendorUser(command.Name, invitation.Email, hashedPassword, invitation.OrganizationId, employeeVendorRole);
                await _vendorRepository.AddAsync(vendorUser);
                newUserId = vendorUser.Id;
            }
            else
            {
                throw new InvalidOperationException("Employee invitation has no valid role assigned.");
            }
        }
        else
        {
            throw new InvalidOperationException("Invitation has no valid role assigned.");
        }

        //---- Mark the invitation as accepted ----
        invitation.AcceptInvitation();
        await _invitationRepository.UpdateAsync(invitation);

        return newUserId;
    }
}
