using Custodian.Application.Common.Exceptions;
using Custodian.Application.Common.Interfaces;
using Custodian.Application.Common.Security;
using Custodian.Domain.Entities;
using Custodian.Domain.Enums;
using Custodian.Domain.Interfaces;
using MediatR;

namespace Custodian.Application.Features.Invitations.InviteCompany.Commands;

public class InviteCompanyCommandHandler : IRequestHandler<InviteCompanyCommand, Guid>
{
    private readonly IVendorUserRepository _vendorUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IOrganizationConnectionRepository _connectionRepository;
    private readonly IEmailSender _emailSender;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ICurrentUserService _currentUserService;

    public InviteCompanyCommandHandler(
        IVendorUserRepository vendorUserRepository,
        IUserRepository userRepository,
        IInvitationRepository invitationRepository,
        IOrganizationConnectionRepository connectionRepository,
        IEmailSender emailSender,
        IAuditLogRepository auditLogRepository,
        ICurrentUserService currentUserService)
    {
        _vendorUserRepository   = vendorUserRepository;
        _userRepository         = userRepository;
        _invitationRepository   = invitationRepository;
        _connectionRepository   = connectionRepository;
        _emailSender            = emailSender;
        _auditLogRepository     = auditLogRepository;
        _currentUserService     = currentUserService;
    }

    public async Task<Guid> Handle(InviteCompanyCommand request, CancellationToken cancellationToken)
    {
        //---- Extract inviter user ID from JWT context ----
        var invitedById = _currentUserService.UserId;
        if (invitedById == Guid.Empty)
            throw new UnauthorizedAccessException("User is not authenticated.");

        //---- Fetch inviter to get OrganizationId ----
        var inviter = await _vendorUserRepository.GetByIdAsync(invitedById)
            ?? throw new NotFoundException($"Vendor user with ID '{invitedById}' was not found.");

        //---- Check if user already exists ----
        var existingUser = await _userRepository.GetByEmailAsync(request.email);
        if (existingUser != null)
        {
            //---- Buyer is existing user's Org, Seller is Inviter Vendor Org ----
            var existingConnection = await _connectionRepository.GetConnectionAsync(existingUser.OrganizationId, inviter.OrganizationId);
            if (existingConnection != null)
            {
                throw new ConflictException("A connection between your organization and this company already exists.");
            }

            var connection = OrganizationConnection.CreateConnection(
                buyerOrganizationId: existingUser.OrganizationId,
                sellerOrganizationId: inviter.OrganizationId,
                requestedById: invitedById,
                paymentTermDays: 30
            );

            await _connectionRepository.AddAsync(connection);
            
            string connectionSubject = $"{inviter.Organization?.Name ?? "A vendor organization"} wants to connect with you on Custodian";
            string connectionBody = $@"
                <h2>New Connection Request on Custodian</h2>
                <p><strong>{inviter.Organization?.Name ?? "A vendor organization"}</strong> has sent you a connection request on Custodian.</p>
                <p>Log in to your account dashboard to view and manage your connections.</p>";
            await _emailSender.SendEmailAsync(request.email, connectionSubject, connectionBody, cancellationToken);

            var connAuditLog = AuditLog.Create(AuditAction.Created, AuditTargetType.OrganizationConnection, connection.Id, invitedById);
            await _auditLogRepository.AddAsync(connAuditLog);

            return connection.Id;
        }

        //---- Check for active pending invitation to avoid duplicate invites ----
        var existingInvitation = await _invitationRepository.GetByEmailAsync(request.email);
        if (existingInvitation != null && !existingInvitation.IsExpired() && !existingInvitation.IsAccepted())
        {
            throw new ConflictException($"An active invitation has already been sent to '{request.email}'.");
        }

        //---- Generate token and create company invitation ----
        var token = TokenGenerator.GenerateInvitationToken();

        var invitation = Invitation.CreateForInternalUser(
            email: request.email,
            token: token,
            userRole: InternalUserRole.Admin,
            organizationId: inviter.OrganizationId,
            invitedById: invitedById,
            invitationType: InvitationType.Company
        );

        string inviteSubject = "You have been invited to join Custodian";
        string inviteBody = $@"
            <h2>Welcome to Custodian!</h2>
            <p>You have been invited to register your company on the platform.</p>
            <p>Use token <strong>{token}</strong> to accept your invitation and set up your account.</p>";
        await _emailSender.SendEmailAsync(request.email, inviteSubject, inviteBody, cancellationToken);
        await _invitationRepository.AddAsync(invitation, cancellationToken);

        var auditLog = AuditLog.Create(AuditAction.Created, AuditTargetType.Invitation, invitation.Id, invitedById);
        await _auditLogRepository.AddAsync(auditLog);

        return invitation.Id;
    }
}
