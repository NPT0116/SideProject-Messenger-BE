using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.Friendship.RejectFriendshipInvitation;

public record RejectFriendshipInvitationCommand(Guid friendshipId) : IRequest;
