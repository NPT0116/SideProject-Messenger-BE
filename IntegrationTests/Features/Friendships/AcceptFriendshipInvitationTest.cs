using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Features.Friendship.GetFriendList;
using Application.Features.Friendship.SendFriendshipInvitation;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Identity;
using IntegrationTests.Shared;

namespace IntegrationTests.Features.Friendships
{
    public class AcceptFriendshipInvitationTest : BaseIntegrationTest
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _dbContext;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly Guid userId = Guid.NewGuid();
        
        public AcceptFriendshipInvitationTest(CustomWebApplicationFactory factory)
            : base(factory)
        {
            _client = factory.CreateClient();
            _dbContext = factory._dbContext;
            _friendshipRepository = factory._friendshipRepository;
        }

        [Fact]
        public async Task AcceptFriendship_ShouldUpdateFriendshipStatus()
        {
            var receiverId = userId;
            var initiatorId = Guid.NewGuid();
            SetupAuthentication(_client, userId);

            var seedUsers = new List<ApplicationUser>
            {
                new ApplicationUser(initiatorId, "Nguyen", "Hong Quan"),
                new ApplicationUser(receiverId, "Nguyen", "Phuc Thanh")
            };

            await _dbContext.Users.AddRangeAsync(seedUsers);
            await _dbContext.SaveChangesAsync();

            var createResponse = await _client.PostAsync(
                $"/api/Friendship?initiatorId={initiatorId}&receiverId={receiverId}", 
               null);
            createResponse.EnsureSuccessStatusCode();

            var result = await createResponse.Content.ReadFromJsonAsync<CreateFriendshipListResponseDto>();
            var friendshipId = result.friendshipId;
            var acceptResponse = await _client.PatchAsync(
                $"/api/Friendship/approve?friendshipId={friendshipId}", 
                null);
            acceptResponse.EnsureSuccessStatusCode();

            var friendship = await _friendshipRepository.GetFriendshipById(friendshipId);
            friendship.Should().NotBeNull();
            friendship.Status.Should().Be(FriendshipStatus.Approved);
            friendship.ReceiverId.Should().Be(receiverId.ToString());
            friendship.InitiatorId.Should().Be(initiatorId.ToString());
        }

        [Fact]
        public async Task AcceptFriendship_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            ClearAuthentication(_client);

            var friendshipId = Guid.NewGuid();
            var acceptResponse = await _client.PatchAsync(
                $"/api/Friendship/approve?friendshipId={friendshipId}", 
                null);
            acceptResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AcceptFriendship_ShouldReturnNotFound_WhenFriendshipNotFound()
        {
            SetupAuthentication(_client, userId);
            var friendshipId = Guid.NewGuid();
            var acceptResponse = await _client.PatchAsync(
                $"/api/Friendship/approve?friendshipId={friendshipId}", 
                null);
            acceptResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AcceptFriendship_ShouldReturnFriendshipNotMatch_WhenUserIsNotReceiver()
        {
            var initiatorId = userId;
            var receiverId = Guid.NewGuid();
            SetupAuthentication(_client, userId);

            var seedUsers = new List<ApplicationUser>
            {
                new ApplicationUser(initiatorId, "Nguyen", "Hong Quan"),
                new ApplicationUser(receiverId, "Nguyen", "Phuc Thanh")
            };

            await _dbContext.Users.AddRangeAsync(seedUsers);
            await _dbContext.SaveChangesAsync();

            var createResponse = await _client.PostAsync(
                $"/api/Friendship?initiatorId={initiatorId}&receiverId={receiverId}", 
               null);
            createResponse.EnsureSuccessStatusCode();

            var result = await createResponse.Content.ReadFromJsonAsync<CreateFriendshipListResponseDto>();
            var friendshipId = result.friendshipId;
            var acceptResponse = await _client.PatchAsync(
                $"/api/Friendship/approve?friendshipId={friendshipId}", 
                null);
            acceptResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

        }
    }
}