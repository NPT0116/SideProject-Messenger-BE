using Application.Dtos.Users;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequestDto request)
    {
        var result = await _mediator.Send(new LoginUserCommand(request));
        return Ok(result);
    }
}

