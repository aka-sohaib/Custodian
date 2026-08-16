using MediatR;

namespace Custodian.Application.Features.Invitations.InviteCompany.Commands;

public record InviteCompanyCommand(
        string email
    ) : IRequest<Guid>;
