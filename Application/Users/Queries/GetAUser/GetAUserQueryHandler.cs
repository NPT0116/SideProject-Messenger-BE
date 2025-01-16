using System;
using Application.Users.Queries.GetAllUsers;
using Domain.Exceptions.Users;
using Domain.Repositories;
using MediatR;

namespace Application.Users.Queries.GetAUser;

public class GetAUserQueryHandler : IRequestHandler<GetAUserQuery, UserDto>
{
        private readonly IUserRepository _userRepository;
        public GetAUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    public Task<UserDto> Handle(GetAUserQuery request, CancellationToken cancellationToken)
    {
        var user = _userRepository.GetUserById(request.Id);
        if (user == null)
        {
            throw new UserNotFound(request.Id);
        }

        return Task.FromResult(new UserDto(user.Id, user.FirstName, user.LastName));
    }
}
