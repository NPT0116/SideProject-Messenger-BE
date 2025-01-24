using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Domain;
using Domain.Exceptions.Friendships;

namespace Application.Features.Friendship.AcceptFriendshipInvitation
{
    public class AcceptFriendshipInvitationCommandHandler : IRequestHandler<AcceptFriendshipInvitationCommand>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public AcceptFriendshipInvitationCommandHandler(
            IFriendshipRepository friendshipRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _friendshipRepository = friendshipRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Handle(AcceptFriendshipInvitationCommand request, CancellationToken cancellationToken)
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

            await _friendshipRepository.UpdateFriendshipStatusById(friendshipId, FriendshipStatus.Approved);
        }
    }
}