using System;
using Application.Dtos.Users;
using Domain;
using Domain.Exceptions.Users;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.Features.Auth.Login;

public class LoginUserCommandHandler: IRequestHandler<LoginUserCommand, Response<LoginUserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    public LoginUserCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<Response<LoginUserResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUsernameAsync(request.LoginRequest.UserName);
        Console.WriteLine("Username: " + request.LoginRequest.UserName);
        Console.WriteLine("Password: " + request.LoginRequest.Password);
        if (user == null)
        {
            throw new UserNotFound("Invalid username or password");
        }

        if (! await _userRepository.checkPasswordAsync(user, request.LoginRequest.Password))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        var token = _tokenService.CreateUserToken(user);
        var responseDto = new LoginUserResponseDto
            {
                UserName = user.UserName,
                Token = token
            };
        return new Response<LoginUserResponseDto>(responseDto)
        {
            Message = "Login successful"
        };
    }
}
