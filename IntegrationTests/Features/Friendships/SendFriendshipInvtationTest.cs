using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Friendship.SendFriendshipInvitation;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions.Friendships;
using Domain.Exceptions.Users;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Identity;
using IntegrationTests.Shared;

namespace IntegrationTests.Features.Friendships
{
    public class SendFriendshipInvtationTest : BaseIntegrationTest
    {
        private readonly Guid userId = Guid.NewGuid();
        private readonly ApplicationDbContext _context;
        public SendFriendshipInvtationTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _context = factory._dbContext;
        }

        private async Task<List<ApplicationUser>> SetupInitiatorAndReceiver()
        {
            var initiator = new ApplicationUser(userId, "Nguyen", "Hong Quan");
            var receiver = new ApplicationUser(Guid.NewGuid(), "Nguyen", "Phuc Thanh");
            await _context.Users.AddRangeAsync(initiator, receiver);
            await _context.SaveChangesAsync();

            return new List<ApplicationUser> {initiator, receiver};
        }

        [Fact]
        public async Task SendFriendshipInvitation_ShouldThrowUserNotFound_WhenInitiatorDoesNotExist()
        {
            await Assert.ThrowsAsync<UserNotFound>(() => _sender.Send(new SendFriendshipInvitationCommand(new CreateFriendshipDto(
                Guid.NewGuid(), userId))));
        }

        [Fact]
        public async Task SendFriendshipInvitation_ShouldThrowUserNotFound_WhenReceiverDoesNotExist()
        {
            await Assert.ThrowsAsync<UserNotFound>(() => _sender.Send(new SendFriendshipInvitationCommand(new CreateFriendshipDto(
                userId, Guid.NewGuid()))));
        }

        [Fact]
        public async Task SendFriendshipInvitation_ShouldThrowHasAlreadyBeenFriend_WhenFriendshipAlreadyExists()
        {
            var users = await SetupInitiatorAndReceiver();
            var initiatorId = users[0].Id;
            var receiverId = users[0].Id;

            var friendship = new Friendship(
                Guid.NewGuid(),
                initiatorId.ToString(),
                receiverId.ToString()
            );
            friendship.SetFriendshipStatus(FriendshipStatus.Approved);

            await _context.Friendships.AddAsync(friendship);
            await _context.SaveChangesAsync();

            var anotherFriendship = new Friendship(
                Guid.NewGuid(),
                initiatorId.ToString(),
                receiverId.ToString()
            );

            await Assert.ThrowsAsync<HasAlreadyBeenFriend>(() => _sender.Send(new SendFriendshipInvitationCommand(new CreateFriendshipDto(
                Guid.Parse(initiatorId), 
                Guid.Parse(receiverId)))));
        }

        [Fact]
        public async Task SendFriendshipInvitation_ShouldSuccess_WhenFriendshipDoesNotExist()
        {
            var users = await SetupInitiatorAndReceiver();
            var initiatorId = users[0].Id;
            var receiverId = users[0].Id;


            var responseDto = await _sender.Send(new SendFriendshipInvitationCommand(new CreateFriendshipDto(
                Guid.Parse(initiatorId), 
                Guid.Parse(receiverId))));
            responseDto.friendshipId.Should().NotBe(Guid.Empty);
            responseDto.initiatorId.Should().Be(initiatorId.ToString());
            responseDto.receiverId.Should().Be(receiverId.ToString());

            var friendshipId = responseDto.friendshipId;
            var friendship = await _context.Friendships.FindAsync(friendshipId);

            friendship.Should().NotBeNull();
            friendship.InitiatorId.Should().Be(initiatorId.ToString());
            friendship.ReceiverId.Should().Be(receiverId.ToString());    
            friendship.Status.Should().Be(FriendshipStatus.Pending);
        }

        [Fact]
        public async Task SendFriendshipInvitation_ShouldSuccess_WhenFriendshipIsRejected()
        {
            var users = await SetupInitiatorAndReceiver();
            var initiatorId = users[0].Id;
            var receiverId = users[0].Id;

            var friendship = new Friendship(
                Guid.NewGuid(),
                initiatorId.ToString(),
                receiverId.ToString()
            );
            friendship.SetFriendshipStatus(FriendshipStatus.Rejected);
            await _context.Friendships.AddRangeAsync(friendship);
            await _context.SaveChangesAsync();

            var responseDto = await _sender.Send(new SendFriendshipInvitationCommand(new CreateFriendshipDto(
                Guid.Parse(initiatorId), 
                Guid.Parse(receiverId))));
            responseDto.friendshipId.Should().NotBe(Guid.Empty);
            responseDto.initiatorId.Should().Be(initiatorId.ToString());
            responseDto.receiverId.Should().Be(receiverId.ToString());

            var friendshipId = responseDto.friendshipId;
            var newFriendship = await _context.Friendships.FindAsync(friendshipId);

            newFriendship.Should().NotBeNull();
            newFriendship.InitiatorId.Should().Be(initiatorId.ToString());
            newFriendship.ReceiverId.Should().Be(receiverId.ToString());    
            newFriendship.Status.Should().Be(FriendshipStatus.Pending);
        }   
    }
}