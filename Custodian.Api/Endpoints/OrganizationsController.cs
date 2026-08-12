using Custodian.Application.Features.Organizations.Commands.RegisterOrganization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Custodian.Api.Endpoints;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterOrganization([FromBody] RegisterOrganizationCommand command)
    {
        var organizationId = await _mediator.Send(command);
        return Created($"/api/organizations/{organizationId}", new
        {
            Message = "Organization & Admin User Registered Successfully!",
            OrganizationId = organizationId
        });
    }
}
