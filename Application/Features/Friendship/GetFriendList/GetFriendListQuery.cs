using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Dtos.Friendship;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Friendship.GetFriendList;
public enum FriendshipFilter
{
    Initiated,
    Received,
    Both
}

public record GetFriendListQuery(Guid userId, FriendshipStatus? status, FriendshipFilter filter) : IRequest<GetFriendListQueryResponse>;

public record UserGetFriendListDto(Guid id, string userName, string firstName, string lastName, string profilePicture);
public record FriendshipDto(Guid id, UserGetFriendListDto initiator, UserGetFriendListDto receiver, FriendshipStatus status, DateTime createdAt);
public record GetFriendListQueryResponse(List<FriendshipDto> friends);

