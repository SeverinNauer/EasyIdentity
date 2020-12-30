using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var cmd = new SignupCommand(new ProjectData("", ""), request.EmailAddress, request.Password, request.Username);
            var result = await mediator.Send(cmd);
            return result.Match<IActionResult>(user => Ok(), BadRequest);
        }
    }
}