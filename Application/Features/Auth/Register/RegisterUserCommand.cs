using System;
using Application.Dtos.Users;
using Domain.Utils;
using MediatR;

namespace Application.Features.Auth.Register;


public record RegisterUserCommand(RegisterUserRequestDto request): IRequest<Response<RegisterUserResponseDto>>;

