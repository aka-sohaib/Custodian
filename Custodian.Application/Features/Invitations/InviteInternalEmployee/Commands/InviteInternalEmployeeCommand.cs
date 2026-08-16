using Custodian.Domain.Enums;
using MediatR;

namespace Custodian.Application.Features.Invitations.InviteInternalEmployee.Commands;

public record InviteInternalEmployeeCommand(
    string email,
    InternalUserRole role
) : IRequest<Guid>;
