using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Dtos.Friendship;
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

public record GetFriendListQueryResponse(List<FriendshipResponseDto> friends);

