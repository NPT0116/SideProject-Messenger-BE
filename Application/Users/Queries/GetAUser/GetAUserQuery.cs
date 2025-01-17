using System;
using Application.Users.Queries.GetAllUsers;
using Domain.Utils;
using MediatR;

namespace Application.Users.Queries.GetAUser;


public record GetAUserQuery(Guid Id): IRequest<Response<UserDto>>;