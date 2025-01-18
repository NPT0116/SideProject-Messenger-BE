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
            var friends = await _friendshipRepository.GetFriendList(request.userId);
            var friendDtos = friends.Select(fr => new Friend(fr.Id, fr.UserName, fr.FirstName, fr.LastName, fr.ProfilePictureId)
            ).ToList();

            return new GetFriendListQueryResponse(friendDtos);
        }
    }
}

//public record Friend(Guid id, string Username, string FirstName, string LastName, Guid ProfilePictureId);
