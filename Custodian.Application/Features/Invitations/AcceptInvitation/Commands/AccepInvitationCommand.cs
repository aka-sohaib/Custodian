using MediatR;

namespace Custodian.Application.Features.Invitations.AcceptInvitation.Commands;

public record AccepInvitationCommand(string Name, string Token, string Password) : IRequest<Guid>;
