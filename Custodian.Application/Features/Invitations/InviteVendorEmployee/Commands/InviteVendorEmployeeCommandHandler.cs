using Custodian.Application.Common.Exceptions;
using Custodian.Application.Common.Interfaces;
using Custodian.Application.Common.Security;
using Custodian.Domain.Entities;
using Custodian.Domain.Interfaces;
using MediatR;

namespace Custodian.Application.Features.Invitations.InviteVendorEmployee.Commands;

public class InviteVendorEmployeeCommandHandler : IRequestHandler<InviteVendorEmployeeCommand, Guid>
{
    private readonly IVendorUserRepository _vendorUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IEmailSender _emailSender;

    public InviteVendorEmployeeCommandHandler(
        IVendorUserRepository vendorUserRepository,
        IUserRepository userRepository,
        IInvitationRepository invitationRepository,
        IEmailSender emailSender)
    {
        _vendorUserRepository = vendorUserRepository;
        _userRepository       = userRepository;
        _invitationRepository = invitationRepository;
        _emailSender          = emailSender;
    }

    public async Task<Guid> Handle(InviteVendorEmployeeCommand request, CancellationToken cancellationToken)
    {
        //---- Fetch inviter to get OrganizationId ----
        var inviter = await _vendorUserRepository.GetByIdAsync(request.invitedById)
            ?? throw new NotFoundException($"Vendor user with ID '{request.invitedById}' was not found.");

        //---- Check if invited user already exists ----
        bool isEmailUnique = await _userRepository.IsEmailUniqueAsync(request.email);
        if (!isEmailUnique) { throw new ConflictException($"User with email '{request.email}' already exists."); }

        //---- Generate token ----
        var token = TokenGenerator.GenerateInvitationToken();

        //---- Create entity ----
        var invitation = Invitation.CreateForVendorUser(
            email: request.email,
            token: token,
            userRole: request.role,
            organizationId: inviter.OrganizationId,
            invitedById: request.invitedById
        );

        //---- Send email ----
        await _emailSender.SendInvitationEmailAsync(request.email, token, cancellationToken);

        //---- Save via repository ----
        await _invitationRepository.AddAsync(invitation, cancellationToken);

        return invitation.Id;
    }
}
