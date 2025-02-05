using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Domain.Dtos.Friendship;
using Domain.Dtos.Shared;
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
            var response = new PageResponseDto<FriendshipListResponseDto>();
            switch(request.filter)
            {
                case FriendshipFilter.Initiated:
                    response = await _friendshipRepository.GetInitiatedFriendList(request.userId, request.status, request.pageNumber, request.pageSize);
                    break;
                case FriendshipFilter.Received:
                    response = await _friendshipRepository.GetReceivedFriendList(request.userId, request.status, request.pageNumber, request.pageSize);
                    break;
                case FriendshipFilter.Both:
                    response = await _friendshipRepository.GetFriendList(request.userId, request.status, request.pageNumber, request.pageSize);
                    break;
            }

            return new GetFriendListQueryResponse(new PageResponseDto<FriendshipDto>
            {
                Data = response.Data.Select(d => new FriendshipDto(
                    d.Friendship.Id, 
                    new UserGetFriendListDto(d.Initiator.Id, d.Initiator.UserName, d.Initiator.FirstName, d.Initiator.LastName, d.Initiator.ProfilePictureId.ToString()),
                    new UserGetFriendListDto(d.Receiver.Id, d.Receiver.UserName, d.Receiver.FirstName, d.Receiver.LastName, d.Receiver.ProfilePictureId.ToString()),  
                    d.Friendship.Status, 
                    d.Friendship.CreatedAt)).ToList(),
                PageNumber = response.PageNumber,
                PageSize = response.PageSize,
                TotalRecords = response.TotalRecords,
                TotalPages = response.TotalPages
            });
            
            // var transformedFriends = friends.Select(friend => new FriendshipDto(
            //     friend.Friendship.Id,
            //     new UserGetFriendListDto(
            //         friend.Initiator.Id,
            //         friend.Initiator.UserName,
            //         friend.Initiator.FirstName,
            //         friend.Initiator.LastName,
            //         friend.Initiator.ProfilePictureId.ToString() ?? ""
            //     ),
            //     new UserGetFriendListDto(
            //         friend.Receiver.Id,
            //         friend.Receiver.UserName,
            //         friend.Receiver.FirstName,
            //         friend.Receiver.LastName,
            //         friend.Receiver.ProfilePictureId.ToString() ?? ""
            //     ),
            //     friend.Friendship.Status,
            //     friend.Friendship.CreatedAt
            // )).ToList();
            
            // return new GetFriendListQueryResponse(new Domain.Dtos.Shared.PageResponseDto<FriendshipDto>
            // {
            //     Data = transformedFriends
            // });
        }
    }
}

//public record Friend(Guid id, string Username, string FirstName, string LastName, Guid ProfilePictureId);
