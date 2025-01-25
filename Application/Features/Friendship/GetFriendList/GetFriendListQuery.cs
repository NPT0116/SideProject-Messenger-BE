using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using MediatR;

namespace Application.Features.Friendship.GetFriendList;

public record GetFriendListQuery(Guid userId, FriendshipStatus? status) : IRequest<GetFriendListQueryResponse>;

public record GetFriendListQueryResponse(List<Friend> friends);
public record Friend(Guid id, string Username, string FirstName, string LastName, Guid? ProfilePictureId);
