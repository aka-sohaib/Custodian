using Custodian.Application.Features.Invitations.AcceptInvitation.Commands;
using Custodian.Application.Features.Invitations.InviteCompany.Commands;
using Custodian.Application.Features.Invitations.InviteInternalEmployee.Commands;
using Custodian.Application.Features.Invitations.InviteVendor.Commands;
using Custodian.Application.Features.Invitations.InviteVendorEmployee.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Custodian.Api.Endpoints;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InvitationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvitationsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("internal-employee")]
    public async Task<IActionResult> InviteInternalEmployee([FromBody] InviteInternalEmployeeCommand command, CancellationToken cancellationToken)
    {
        var invitationId = await _mediator.Send(command, cancellationToken);
        return Created($"/api/invitations/{invitationId}", new
        {
            Message = "Internal employee invitation sent successfully.",
            InvitationId = invitationId
        });
    }

    [HttpPost("vendor")]
    public async Task<IActionResult> InviteVendor([FromBody] InviteVendorCommand command, CancellationToken cancellationToken)
    {
        var invitationId = await _mediator.Send(command, cancellationToken);
        return Created($"/api/invitations/{invitationId}", new
        {
            Message = "Vendor invitation sent successfully.",
            InvitationId = invitationId
        });
    }

    [HttpPost("company")]
    public async Task<IActionResult> InviteCompany([FromBody] InviteCompanyCommand command, CancellationToken cancellationToken)
    {
        var invitationId = await _mediator.Send(command, cancellationToken);
        return Created($"/api/invitations/{invitationId}", new
        {
            Message = "Company invitation sent successfully.",
            InvitationId = invitationId
        });
    }

    [HttpPost("vendor-employee")]
    public async Task<IActionResult> InviteVendorEmployee([FromBody] InviteVendorEmployeeCommand command, CancellationToken cancellationToken)
    {
        var invitationId = await _mediator.Send(command, cancellationToken);
        return Created($"/api/invitations/{invitationId}", new
        {
            Message = "Vendor employee invitation sent successfully.",
            InvitationId = invitationId
        });
    }

    [AllowAnonymous]
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptInvitation([FromBody] AccepInvitationCommand command, CancellationToken cancellationToken)
    {
        var userId = await _mediator.Send(command,cancellationToken);
        return Ok(new { UserId = userId });
    }
}
