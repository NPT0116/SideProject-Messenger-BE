using System;
using Application.Dtos.Users;
using MediatR;

namespace Application.Features.Auth.Register;


public record RegisterUserCommand(RegisterUserRequestDto request): IRequest<RegisterUserResponseDto>;

