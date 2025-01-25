using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
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
            var friends = await _friendshipRepository.GetFriendList(request.userId, request.status);
            
            return new GetFriendListQueryResponse(friends);
        }
    }
}

//public record Friend(Guid id, string Username, string FirstName, string LastName, Guid ProfilePictureId);
