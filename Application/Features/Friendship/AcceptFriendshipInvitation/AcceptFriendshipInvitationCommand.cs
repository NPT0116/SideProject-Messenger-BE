using MediatR;

namespace Application.Features.Friendship.AcceptFriendshipInvitation;

public record AcceptFriendshipInvitationCommand(Guid friendshipId) : IRequest;
