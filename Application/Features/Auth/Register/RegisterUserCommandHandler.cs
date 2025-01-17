using System;
using Application.Dtos.Users;
using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;

namespace Application.Features.Auth.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Response<RegisterUserResponseDto>>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Response<RegisterUserResponseDto>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.request;

        var user = new User(
            request.UserName,
            request.FirstName,
            request.LastName,
            request.Password
        );
        
        var createdUser = await _userRepository.CreateUserAsync(user);
        user.SetId(createdUser.Id);

        return new Response<RegisterUserResponseDto>( new RegisterUserResponseDto
        {
            UserName = user.UserName,
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName
        })
        {
            Message = "User created successfully"
        };
    }
}