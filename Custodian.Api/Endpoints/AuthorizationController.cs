using Custodian.Application.Features.Users.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Custodian.Api.Endpoints
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController: ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthorizationController(IMediator mediator)=> _mediator = mediator;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand)
        {
            var response = await _mediator.Send(loginCommand);
            return Ok(response);
        }
    }
}
