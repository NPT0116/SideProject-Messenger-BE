using System;
using Application.Dtos.Users;
using Domain.Utils;
using MediatR;

namespace Application.Features.Auth.Login;

public record LoginUserCommand (LoginUserRequestDto LoginRequest): IRequest<Response<LoginUserResponseDto>>;
