using Application.Dtos.Users;
using Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto request)
    {

        var result = await _mediator.Send(new RegisterUserCommand (request ));
        return Ok(result);
    }

}

