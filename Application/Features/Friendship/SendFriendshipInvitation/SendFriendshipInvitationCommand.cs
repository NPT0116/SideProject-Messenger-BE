using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.Friendship.SendFriendshipInvitation;

public record SendFriendshipInvitationCommand(CreateFriendshipDto request) : IRequest<CreateFriendshipListResponseDto>;

public record CreateFriendshipDto(Guid initiatorId, Guid receiverId);
public record CreateFriendshipListResponseDto(Guid friendshipId, Guid initiatorId, Guid receiverId);