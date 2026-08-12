using Custodian.Domain.Enums;
using MediatR;

namespace Custodian.Application.Features.Invitations.InviteVendorEmployee.Commands;

public record InviteVendorEmployeeCommand(
        string email,
        VendorUserRole role,
        Guid invitedById
    ) : IRequest<Guid>;

