using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Features.Friendship.AcceptFriendshipInvitation;
using Domain.Entities;
using Domain.Exceptions.Friendships;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.Tests.Friendships
{
    public class AcceptFriendshipInvitationCommandTest
    {
        private readonly Mock<IFriendshipRepository> _friendshipRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly AcceptFriendshipInvitationCommandHandler _handler;
        public AcceptFriendshipInvitationCommandTest()
        {
            _friendshipRepositoryMock = new Mock<IFriendshipRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _handler = new AcceptFriendshipInvitationCommandHandler(_friendshipRepositoryMock.Object, _httpContextAccessorMock.Object);
        }

        private void SetupHttpContext(string userId)
        {
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
            var context = new DefaultHttpContext { User = claims };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsNull()
        {
            // Arrange
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
            var command = new AcceptFriendshipInvitationCommand(Guid.NewGuid());

            // Act
            Func<Task> act = async () => await _handler.Handle(command, default);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        }

        [Fact]
        public async Task Handle_ShouldThrowFriendshipNotFound_WhenFriendshipDoesNotExist()
        {
            var friendshipId = Guid.NewGuid();
            SetupHttpContext("user-id");

            _friendshipRepositoryMock
                .Setup(repo => repo.GetFriendshipById(friendshipId))
                .ReturnsAsync((Friendship?)null);

            var command = new AcceptFriendshipInvitationCommand(friendshipId);

            await Assert.ThrowsAsync<FriendshipNotFound>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ShouldThrowFriendshipDoesNotMatch_WhenUserIsNotReceiver()
        {
            var friendshipId = Guid.NewGuid();
            var initiatorId = "initiator-id";
            var receiverId = "receiver-id";
            SetupHttpContext(initiatorId);
            _friendshipRepositoryMock
                .Setup(repo => repo.GetFriendshipById(friendshipId))
                .ReturnsAsync(new Friendship(Guid.NewGuid(), It.IsAny<string>(), receiverId));

            var command = new AcceptFriendshipInvitationCommand(friendshipId);

            await Assert.ThrowsAsync<FriendshipNotMatch>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ShouldAcceptFriendship_WhenUserIsReceiver()
        {
            var friendshipId = Guid.NewGuid();
            var receiverId = "receiver-id";
            SetupHttpContext(receiverId);

            _friendshipRepositoryMock
                .Setup(repo => repo.GetFriendshipById(friendshipId))
                .ReturnsAsync(new Friendship(Guid.NewGuid(), It.IsAny<string>(), receiverId));

            _friendshipRepositoryMock
                .Setup(repo => repo.UpdateFriendshipStatusById(friendshipId, Domain.Enums.FriendshipStatus.Approved))
                .Returns(Task.CompletedTask);

            var command = new AcceptFriendshipInvitationCommand(friendshipId);

            await _handler.Handle(command, default);

            _friendshipRepositoryMock.Verify(repo => repo.UpdateFriendshipStatusById(friendshipId, Domain.Enums.FriendshipStatus.Approved), Times.Once);
        }
    }
}