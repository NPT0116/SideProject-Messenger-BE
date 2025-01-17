using Application.Users.Queries.GetAllUsers;
using Application.Users.Queries.GetAUser;
using Application.Users.Queries.GetMeUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            // Send the query to MediatR
            var response = await _mediator.Send(new GetAllUsersQuery());

            // Return the response
            return Ok(response.Users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAUser(Guid id)
        {
            // Send the query to MediatR
            var response = await _mediator.Send(new GetAUserQuery(id));

            // Return the response
            return Ok(response);
        }
    
    [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            // Get the current user id
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // Log.Information(token);
            // Send the query to MediatR
            var response = await _mediator.Send(new GetMeUserCommand(token));

            // Return the response
            return Ok(response);
        }
    }
}
