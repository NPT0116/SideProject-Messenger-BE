using System;
using Application.Dtos.Users;
using Domain.Utils;
using MediatR;

namespace Application.Users.Queries.GetMeUser;

public record GetMeUserCommand(string Token): IRequest<Response<GetMeUserResponseDto>>;

