using System;
using Application.Users.Queries.GetAllUsers;
using MediatR;

namespace Application.Users.Queries.GetAUser;


public record GetAUserQuery(Guid Id): IRequest<UserDto>;