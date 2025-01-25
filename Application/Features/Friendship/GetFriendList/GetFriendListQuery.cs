using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Dtos.Friendship;
using Domain.Enums;
using MediatR;

namespace Application.Features.Friendship.GetFriendList;

public record GetFriendListQuery(Guid userId, FriendshipStatus? status, bool IsInitiated) : IRequest<GetFriendListQueryResponse>;

public record GetFriendListQueryResponse(List<FriendshipResponseDto> friends);

