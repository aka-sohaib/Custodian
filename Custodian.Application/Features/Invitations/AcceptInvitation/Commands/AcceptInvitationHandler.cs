using Custodian.Application.Common.Interfaces;
using Custodian.Domain.Interfaces;
using Custodian.Domain.Enums;
using MediatR;
using Custodian.Application.Common.Exceptions;
using Custodian.Domain.Entities;

namespace Custodian.Application.Features.Invitations.AcceptInvitation.Commands;

public class AcceptInvitationHandler: IRequestHandler<AccepInvitationCommand, Guid>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IVendorUserRepository _vendorRepository;
    private readonly IInternalUserRepository _internalUserRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AcceptInvitationHandler(IInvitationRepository invitationRepository, IVendorUserRepository vendorRepository, IInternalUserRepository internalUserRepository, IPasswordHasher passwordHasher)
    {
        _invitationRepository = invitationRepository;
        _vendorRepository = vendorRepository;
        _internalUserRepository = internalUserRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(AccepInvitationCommand command, CancellationToken cancellationToken)
    {
        //---- Token Validation ----
        var invitation = await _invitationRepository.GetByTokenAsync(command.Token) ?? 
            throw new NotFoundException("The invitation link is invalid or corrupted.");

        if (invitation.AcceptedAt == null) throw new ConflictException("This invitation has already been used.");
        if (invitation.IsExpired()) throw new ConflictException("This invitation has expired.");

        //---- Hash Password ----
        var hashedPassword = _passwordHasher.Hash(command.Password);

        //---- Check User Type & create entity ----
        Guid newUserId = Guid.Empty;

        if (invitation.InvitationType == InvitationType.Vendor && invitation.VendorUserRole is VendorUserRole vendorUserRole)
        {
            var vendorUser = VendorUser.CreateVendorUser(command.Name, invitation.Email, hashedPassword, invitation.OrganizationId, vendorUserRole);
            await _vendorRepository.AddAsync(vendorUser);
            newUserId = vendorUser.Id;
        }
        else if (invitation.InvitationType == InvitationType.Employee && invitation.InternalUserRole is InternalUserRole internalUserRole)
        {
            var internalUser = InternalUser.CreateInternalUser(command.Name, invitation.Email, hashedPassword, internalUserRole, invitation.OrganizationId);
            await _internalUserRepository.AddAsync(internalUser);
            newUserId = internalUser.Id;
        }
        else
        {
            throw new InvalidOperationException("Invitation has no valid role assigned.");
        }

        //---- Mark the invitation as accepted ----
        invitation.AcceptInvitation();

        return newUserId;
    }

}
