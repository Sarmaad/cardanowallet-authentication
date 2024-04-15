using Asp.Versioning;
using Backend.Api.Features.Login.Operations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Features.Login
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LoginController(IMediator mediator) : ControllerBase
    {
        [HttpPost("nonce")]
        public async Task<IActionResult> GenerateNonce([FromBody] GenerateNonce request)
            => Ok(await mediator.Send(request));
    }
}
