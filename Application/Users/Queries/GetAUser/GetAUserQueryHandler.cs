using System;
using Application.Users.Queries.GetAllUsers;
using Domain.Exceptions.Users;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.Users.Queries.GetAUser;

public class GetAUserQueryHandler : IRequestHandler<GetAUserQuery, Response<UserDto>>
{
        private readonly IUserRepository _userRepository;
        public GetAUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    public Task<Response<UserDto>> Handle(GetAUserQuery request, CancellationToken cancellationToken)
    {
        var user =  _userRepository.GetUserById(request.Id);
        if (user == null)
        {
            throw new UserNotFound(request.Id);
        }

        return Task.FromResult( new Response<UserDto>(new UserDto
        (
           user.Id,
            user.UserName,
            user.FirstName,
            user.LastName
        )));

    }
}
