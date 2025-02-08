using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Exceptions.Friendships;
using Domain.Exceptions.Users;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Friendship.SendFriendshipInvitation
{
    public class SendFriendshipInvitationCommandHandler : IRequestHandler<SendFriendshipInvitationCommand, CreateFriendshipListResponseDto>
    {
        private IFriendshipRepository _friendshipRepository;
        private IUserRepository _userRepository;
        public SendFriendshipInvitationCommandHandler(
            IFriendshipRepository friendshipRepository,
            IUserRepository userRepository)
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
        }
        public async Task<CreateFriendshipListResponseDto> Handle(SendFriendshipInvitationCommand command, CancellationToken cancellationToken)
        {
            var initiatorId = command.request.initiatorId;
            var receiverId = command.request.receiverId;

            var initiator = await _userRepository.GetUserByIdAsync(initiatorId);
            if(initiator == null)
            {
                throw new UserNotFound(initiatorId);
            }

            var receiver = await _userRepository.GetUserByIdAsync(receiverId);
            if(receiver == null)
            {
                throw new UserNotFound(receiverId);
            }

            var friendship = await _friendshipRepository.GetFriendshipBetweenTwoUsersByIds(initiatorId, receiverId);

            if(friendship != null && friendship.Status != FriendshipStatus.Rejected)
            {
                throw new HasAlreadyBeenFriend(initiatorId, receiverId);
            }

            var friendshipId = await _friendshipRepository.CreateFriendship(initiatorId, receiverId);

            return new CreateFriendshipListResponseDto(friendshipId, initiatorId, receiverId);
        }
    }
}