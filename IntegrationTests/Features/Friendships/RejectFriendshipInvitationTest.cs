using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Friendship.RejectFriendshipInvitation;
using Domain.Entities;
using Domain.Exceptions.Friendships;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Identity;
using IntegrationTests.Shared;

namespace IntegrationTests.Features.Friendships
{
    public class RejectFriendshipInvitationTest : BaseIntegrationTest
    {
        private readonly Guid userId = Guid.NewGuid();
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;
        public RejectFriendshipInvitationTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _client = factory.CreateClient();
            _context = factory._dbContext;
        }

        

        [Fact]
        public async Task RejectFriendship_ShouldThrowUnauthorizedException_WhenUserIsNotAuthenticated()
        {
            var response = await _client.PatchAsync($"/api/Friendship/reject?friendshipId={Guid.NewGuid()}", null);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RejectFriendship_ShouldThrowFriendshipNotFound_WhenFriendshipDoesNotExist()
        {
            SetupAuthentication(_client, userId);
            var response = await _client.PatchAsync($"/api/Friendship/reject?friendshipId={Guid.NewGuid()}", null);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RejectFriendship_ShouldThrowFriendshipNotMatch_WhenUserIsNotReceiver()
        {
            var initiator = new ApplicationUser(userId, "Nguyen", "Hong Quan");
            var receiver = new ApplicationUser(Guid.NewGuid(), "Nguyen", "Phuc Thanh");
            await _context.Users.AddRangeAsync(initiator, receiver);
            await _context.SaveChangesAsync();
            SetupAuthentication(_client, userId);
            var initiatorId = userId;
            var receiverId = receiver.Id;
            var friendshipId = Guid.NewGuid();
            await _context.Friendships.AddAsync(new Friendship(friendshipId, initiatorId.ToString(), receiverId.ToString()));
            await _context.SaveChangesAsync();

            var response = await _client.PatchAsync($"/api/Friendship/reject?friendshipId={friendshipId}", null);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        }
    }
}