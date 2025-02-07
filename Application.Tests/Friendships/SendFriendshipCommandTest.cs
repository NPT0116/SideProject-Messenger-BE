using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Friendship.SendFriendshipInvitation;
using Domain.Entities;
using Domain.Exceptions.Friendships;
using Domain.Exceptions.Users;
using Domain.Repositories;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class SendFriendshipCommandTest
    {
        private readonly Mock<IFriendshipRepository> _friendshipRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly SendFriendshipInvitationCommandHandler _handler;

        public SendFriendshipCommandTest()
        {
            _friendshipRepositoryMock = new Mock<IFriendshipRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new SendFriendshipInvitationCommandHandler(
                _friendshipRepositoryMock.Object, 
                _userRepositoryMock.Object);
        }
        [Fact]
        public async Task Handle_ShouldThrowUserNotFound_WhenInitiatorDoesNotExist()
        {
            var initiatorId = Guid.NewGuid();

            var command = new SendFriendshipInvitationCommand(new CreateFriendshipDto(
                initiatorId,
                Guid.NewGuid()
            ));

            _userRepositoryMock
                .Setup(repo => repo.GetUserByIdAsync(initiatorId))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UserNotFound>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ShouldThrowUserNotFound_WhenReceiverDoesNotExist()
        {
            var receiverId = Guid.NewGuid();

            var command = new SendFriendshipInvitationCommand(new CreateFriendshipDto(
                Guid.NewGuid(),
                receiverId
            ));

            _userRepositoryMock
                .Setup(repo => repo.GetUserByIdAsync(receiverId))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UserNotFound>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ShouldThrowHasAlreadyBeenFriend_WhenFriendshipExistsWithRejectedStatus()
        {
           var initiatorId = Guid.NewGuid();
           var receiverId = Guid.NewGuid();

           var command = new SendFriendshipInvitationCommand(new CreateFriendshipDto(
               initiatorId,
               receiverId
           ));

           _userRepositoryMock
                .Setup(repo => repo.GetUserByIdAsync(initiatorId))
                .ReturnsAsync(new User());

            _userRepositoryMock
                .Setup(repo => repo.GetUserByIdAsync(receiverId))
                .ReturnsAsync(new User());

            _friendshipRepositoryMock
                .Setup(repo => repo.GetFriendshipBetweenTwoUsersByIds(initiatorId, receiverId))
                .ReturnsAsync(new Friendship(Guid.NewGuid(), initiatorId.ToString(), receiverId.ToString()));

            await Assert.ThrowsAsync<HasAlreadyBeenFriend>(() => _handler.Handle(command, default));

            _friendshipRepositoryMock.Verify(repo => repo.CreateFriendship(It.IsAny<Guid>(),It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCreateFriendshipSuccessfully()
        {
            var initiatorId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            // Arrange
            var command = new SendFriendshipInvitationCommand(new CreateFriendshipDto(initiatorId, receiverId));

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(initiatorId))
                .ReturnsAsync(new User());

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(receiverId))
                .ReturnsAsync(new User());

            _friendshipRepositoryMock.Setup(repo => repo.GetFriendshipBetweenTwoUsersByIds(initiatorId, receiverId))
                .ReturnsAsync((Friendship)null); // No existing friendship

            _friendshipRepositoryMock.Setup(repo => repo.CreateFriendship(initiatorId, receiverId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(initiatorId, result.initiatorId);
            Assert.Equal(receiverId, result.receiverId);

            _friendshipRepositoryMock.Verify(repo => repo.CreateFriendship(initiatorId, receiverId), Times.Once);
        }
    }
}