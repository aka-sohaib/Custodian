using Custodian.Application.Common.Exceptions;
using Custodian.Application.Common.Interfaces;
using Custodian.Application.Common.Security;
using Custodian.Domain.Entities;
using Custodian.Domain.Interfaces;
using MediatR;

namespace Custodian.Application.Features.Invitations.InviteInternalEmployee.Commands;

public class InviteInternalEmployeeCommandHandler : IRequestHandler<InviteInternalEmployeeCommand, Guid>
{
    private readonly IInternalUserRepository _internalUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IEmailSender _emailSender;

    public InviteInternalEmployeeCommandHandler(
        IInternalUserRepository internalUserRepository,
        IUserRepository userRepository,
        IInvitationRepository invitationRepository,
        IEmailSender emailSender)
    {
        _internalUserRepository = internalUserRepository;
        _userRepository         = userRepository;
        _invitationRepository   = invitationRepository;
        _emailSender            = emailSender;
    }

    public async Task<Guid> Handle(InviteInternalEmployeeCommand request, CancellationToken cancellationToken)
    {
        //---- Fetch inviter to get OrganizationId ----
        var inviter = await _internalUserRepository.GetByIdAsync(request.invitedById)
            ?? throw new NotFoundException($"User with ID '{request.invitedById}' was not found.");

        //---- Check if invited user already exists ----
        bool isEmailUnique = await _userRepository.IsEmailUniqueAsync(request.email);
        if (!isEmailUnique) { throw new ConflictException($"User with email '{request.email}' already exists."); }

        //---- Generate token ----
        var token = TokenGenerator.GenerateInvitationToken();

        //---- Create entity ----
        var invitation = Invitation.CreateForInternalUser(
            email: request.email,
            token: token,
            userRole: request.role,
            organizationId: inviter.OrganizationId,
            invitedById: request.invitedById
        );

        //---- send email ----
        await _emailSender.SendInvitationEmailAsync(request.email, token, cancellationToken);

        //---- save new entity ----
        await _invitationRepository.AddAsync(invitation, cancellationToken);

        return invitation.Id;
    }
}
