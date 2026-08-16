using Custodian.Application.Common.Exceptions;
using Custodian.Application.Common.Interfaces;
using Custodian.Application.Common.Security;
using Custodian.Domain.Entities;
using Custodian.Domain.Enums;
using Custodian.Domain.Interfaces;
using MediatR;

namespace Custodian.Application.Features.Invitations.InviteVendor.Commands;

public class InviteVendorCommandHandler : IRequestHandler<InviteVendorCommand, Guid>
{
    private readonly IInternalUserRepository _internalUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IEmailSender _emailSender;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ICurrentUserService _currentUserService;

    public InviteVendorCommandHandler(
        IInternalUserRepository internalUserRepository,
        IUserRepository userRepository,
        IInvitationRepository invitationRepository,
        IEmailSender emailSender,
        IAuditLogRepository auditLogRepository,
        ICurrentUserService currentUserService)
    {
        _internalUserRepository = internalUserRepository;
        _userRepository         = userRepository;
        _invitationRepository   = invitationRepository;
        _emailSender            = emailSender;
        _auditLogRepository     = auditLogRepository;
        _currentUserService     = currentUserService;
    }

    public async Task<Guid> Handle(InviteVendorCommand request, CancellationToken cancellationToken)
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

        //---- Generate secure token ----
        var token = TokenGenerator.GenerateInvitationToken();

        //---- Create entity ----
        var invitation = Invitation.CreateForVendorUser(
            email: request.email,
            token: token,
            userRole: VendorUserRole.Admin,
            organizationId: inviter.OrganizationId,
            invitedById: invitedById
        );

        //---- Send email ----
        string subject = "You have been invited to join Custodian as a Vendor";
        string htmlBody = $@"
            <h2>Welcome to Custodian!</h2>
            <p>You have been invited to register your vendor organization on Custodian.</p>
            <p>Use token <strong>{token}</strong> to accept your invitation and set up your account.</p>";
        await _emailSender.SendEmailAsync(request.email, subject, htmlBody, cancellationToken);

        //---- Save entity ----
        await _invitationRepository.AddAsync(invitation, cancellationToken);

        //---- Record audit log entry ----
        var auditLog = AuditLog.Create(AuditAction.Created, AuditTargetType.Invitation, invitation.Id, invitedById);
        await _auditLogRepository.AddAsync(auditLog);

        return invitation.Id;
    }
}
