﻿using Domain.Repositories;
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

        Task<GetAllUsersResponse> IRequestHandler<GetAllUsersQuery, GetAllUsersResponse>.Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = _userRepository.GetUsers();

            // Map domain entities to 
            var userDtos = users.Select(user => new UserDto(
                user.Id,
                user.FirstName,
                user.LastName
            )).ToList();

            return Task.FromResult(new GetAllUsersResponse(userDtos));
        }
    }
}
