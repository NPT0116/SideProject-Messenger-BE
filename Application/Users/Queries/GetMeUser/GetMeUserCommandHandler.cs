using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Dtos.Users;
using Domain;
using Domain.Exceptions.Users;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.Users.Queries.GetMeUser
{
    public class GetMeUserCommandHandler : IRequestHandler<GetMeUserCommand, Response<GetMeUserResponseDto>>
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;

        public GetMeUserCommandHandler(ITokenService tokenService, IUserRepository userRepository)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        public async Task<Response<GetMeUserResponseDto>> Handle(GetMeUserCommand request, CancellationToken cancellationToken)
        {
            var userId = _tokenService.GetUserIdFromToken(request.Token);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            var domainUser =  await _userRepository.GetUserByIdAsync(userId.Value);
            if (domainUser == null)
            {
                throw new UserNotFound(userId.Value);
            }

            var responseDto = new GetMeUserResponseDto
            {
                Id = domainUser.Id,
                UserName = domainUser.UserName,
                FirstName = domainUser.FirstName,
                LastName = domainUser.LastName
            };

            return new Response<GetMeUserResponseDto>(responseDto)
            {
                Succeeded = true,
                Message = "User retrieved successfully"
            };
        }
    }
}