using System;
using Application.Dtos.Users;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Auth.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponseDto>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<RegisterUserResponseDto> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.request;
        var user = new User(
            request.FirstName,
            request.LastName,
            request.Password
        );

        var createdUser = await _userRepository.CreateUserAsync(user);
        user.SetId(createdUser.Id);

        return new RegisterUserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}