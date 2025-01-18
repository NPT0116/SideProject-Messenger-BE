using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, GetAllUsersResponse>
    {
        private readonly IUserRepository _userRepository;
        public GetAllUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        async Task<GetAllUsersResponse> IRequestHandler<GetAllUsersQuery, GetAllUsersResponse>.Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetUsersAsync();

            // Map domain entities to 
            var userDtos = users.Select(user => new UserDto(
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName
            )).ToList();

            return await Task.FromResult(new GetAllUsersResponse(userDtos));
        }
    }
}
