using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Exceptions.Friendships;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Friendship.RejectFriendshipInvitation
{
    public class RejectFriendshipInvitationCommandHandler : IRequestHandler<RejectFriendshipInvitationCommand>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public RejectFriendshipInvitationCommandHandler(
            IFriendshipRepository friendshipRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _friendshipRepository = friendshipRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Handle(RejectFriendshipInvitationCommand request, CancellationToken cancellationToken)
        {
            var friendshipId = request.friendshipId;
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if(userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            var friendship = await _friendshipRepository.GetFriendshipById(friendshipId);

            if(friendship == null)
            {
                throw new FriendshipNotFound(friendshipId);
            }

            if(friendship.ReceiverId != userId)
            {
                throw new FriendshipNotMatch(friendshipId, userId);
            }

            await _friendshipRepository.UpdateFriendshipStatusById(friendshipId, FriendshipStatus.Rejected);
        }
    }
}