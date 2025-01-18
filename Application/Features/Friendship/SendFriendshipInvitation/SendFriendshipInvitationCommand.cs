using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.Friendship.SendFriendshipInvitation;

public record SendFriendshipInvitationCommand(CreateFriendshipDto request) : IRequest<CreateFriendshipResponseDto>;

public record CreateFriendshipDto(Guid initiatorId, Guid receiverId);
public record CreateFriendshipResponseDto(Guid initiatorId, Guid receiverId);