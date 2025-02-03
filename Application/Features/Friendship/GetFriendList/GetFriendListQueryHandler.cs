using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Domain.Dtos.Friendship;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Friendship.GetFriendList
{
    public class GetFriendListQueryHandler : IRequestHandler<GetFriendListQuery, GetFriendListQueryResponse>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        public GetFriendListQueryHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }
        public async Task<GetFriendListQueryResponse> Handle(GetFriendListQuery request, CancellationToken cancellationToken)
        {
            var friends = new List<FriendshipResponseDto>();
            switch(request.filter)
            {
                case FriendshipFilter.Initiated:
                    friends = await _friendshipRepository.GetInitiatedFriendList(request.userId, request.status);
                    break;
                case FriendshipFilter.Received:
                    friends = await _friendshipRepository.GetReceivedFriendList(request.userId, request.status);
                    break;
                case FriendshipFilter.Both:
                    friends = await _friendshipRepository.GetFriendList(request.userId, request.status);
                    break;
            }
            
            var transformedFriends = friends.Select(friend => new FriendshipDto(
                friend.Friendship.Id,
                new UserGetFriendListDto(
                    friend.Initiator.Id,
                    friend.Initiator.UserName,
                    friend.Initiator.FirstName,
                    friend.Initiator.LastName,
                    friend.Initiator.ProfilePictureId.ToString() ?? ""
                ),
                new UserGetFriendListDto(
                    friend.Receiver.Id,
                    friend.Receiver.UserName,
                    friend.Receiver.FirstName,
                    friend.Receiver.LastName,
                    friend.Receiver.ProfilePictureId.ToString() ?? ""
                ),
                friend.Friendship.Status,
                friend.Friendship.CreatedAt
            )).ToList();
            
            return new GetFriendListQueryResponse(transformedFriends);
        }
    }
}

//public record Friend(Guid id, string Username, string FirstName, string LastName, Guid ProfilePictureId);
