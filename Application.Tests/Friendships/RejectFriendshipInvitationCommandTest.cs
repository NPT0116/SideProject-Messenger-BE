using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Features.Friendship.RejectFriendshipInvitation;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions.Friendships;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.Tests.Friendships
{
    public class RejectFriendshipInvitationCommandTest
    {
        private readonly Mock<IFriendshipRepository> _friendshipRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly RejectFriendshipInvitationCommandHandler _rejectFriendshipInvitationCommandHandler;
        public RejectFriendshipInvitationCommandTest()
        {
            _friendshipRepositoryMock = new();
            _httpContextAccessorMock = new();
            _rejectFriendshipInvitationCommandHandler = new(
                _friendshipRepositoryMock.Object,
                _httpContextAccessorMock.Object);
        }

        private void SetupHttpContext(string userId)
        {
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
            var context = new DefaultHttpContext { User = claims };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenUserIsNull()
        {
            var command = new RejectFriendshipInvitationCommand(Guid.NewGuid());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _rejectFriendshipInvitationCommandHandler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ShouldThrowFriendNotMatch_WhenUserIsNotReceiver()
        {
            var userId = "user-id";
            var anotherUserId = "another-user-id";
            SetupHttpContext(userId);

            _friendshipRepositoryMock
                .Setup(repo => repo.GetFriendshipById(It.IsAny<Guid>()))
                .ReturnsAsync(new Friendship(It.IsAny<Guid>(), It.IsAny<string>(), anotherUserId));

            var command = new RejectFriendshipInvitationCommand(Guid.NewGuid());

            await Assert.ThrowsAsync<FriendshipNotMatch>(() => _rejectFriendshipInvitationCommandHandler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ShouldThrowFriendshipNotFound_WhenFriendshipIsNull()
        {
            var command = new RejectFriendshipInvitationCommand(It.IsAny<Guid>());
            SetupHttpContext("user-id");

            _friendshipRepositoryMock
                .Setup(repo => repo.GetFriendshipById(It.IsAny<Guid>()))
                .ReturnsAsync((Friendship?)null);

            await Assert.ThrowsAsync<FriendshipNotFound>(() => _rejectFriendshipInvitationCommandHandler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ShouldUpdateFriendshipStatusToRejected()
        {
            var userId = "user-id";
            SetupHttpContext(userId);

            var friendshipId = Guid.NewGuid();
            var friendship = new Friendship(friendshipId, userId, userId);

            var command = new RejectFriendshipInvitationCommand(friendshipId);

            _friendshipRepositoryMock
                .Setup(repo => repo.GetFriendshipById(friendshipId))
                .ReturnsAsync(friendship);

            await _rejectFriendshipInvitationCommandHandler.Handle(command, default);

            _friendshipRepositoryMock.Verify(repo => repo.UpdateFriendshipStatusById(friendshipId, FriendshipStatus.Rejected), Times.Once);
        }
    }
}