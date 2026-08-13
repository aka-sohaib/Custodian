using MediatR;

namespace Custodian.Application.Features.Invitations.AcceptInvitation.Commands;

public record AccepInvitationCommand(
    string Name,
    string Token,
    string Password,
    string? OrganizationName = null,
    string? OrganizationPhone = null,
    string? OrganizationEmail = null
) : IRequest<Guid>;
