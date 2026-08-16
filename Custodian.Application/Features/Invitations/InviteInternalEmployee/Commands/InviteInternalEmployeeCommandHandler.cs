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
    private readonly ICurrentUserService _currentUserService;

    public InviteInternalEmployeeCommandHandler(
        IInternalUserRepository internalUserRepository,
        IUserRepository userRepository,
        IInvitationRepository invitationRepository,
        IEmailSender emailSender,
        ICurrentUserService currentUserService)
    {
        _internalUserRepository = internalUserRepository;
        _userRepository         = userRepository;
        _invitationRepository   = invitationRepository;
        _emailSender            = emailSender;
        _currentUserService     = currentUserService;
    }

    public async Task<Guid> Handle(InviteInternalEmployeeCommand request, CancellationToken cancellationToken)
    {
        //---- Extract inviter user ID from JWT context ----
        var invitedById = _currentUserService.UserId;
        if (invitedById == Guid.Empty)
            throw new UnauthorizedAccessException("User is not authenticated.");

        //---- Fetch inviter to get OrganizationId ----
        var inviter = await _internalUserRepository.GetByIdAsync(invitedById)
            ?? throw new NotFoundException($"User with ID '{invitedById}' was not found.");

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
            invitedById: invitedById
        );

        //---- Send email ----
        string subject = "You have been invited to join Custodian";
        string htmlBody = $@"
            <h2>Welcome to Custodian!</h2>
            <p>You have been invited to join the team on Custodian.</p>
            <p>Use token <strong>{token}</strong> to accept your invitation and set up your account.</p>";
        await _emailSender.SendEmailAsync(request.email, subject, htmlBody, cancellationToken);

        //---- Save new entity ----
        await _invitationRepository.AddAsync(invitation, cancellationToken);

        return invitation.Id;
    }
}
