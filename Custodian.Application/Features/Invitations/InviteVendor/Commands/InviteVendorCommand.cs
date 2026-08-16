using Custodian.Domain.Enums;
using MediatR;

namespace Custodian.Application.Features.Invitations.InviteVendor.Commands;

public record InviteVendorCommand(
        string email
    ) : IRequest<Guid>;
