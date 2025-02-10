using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Features.Friendship.GetFriendList;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Identity;
using IntegrationTests.Shared;

namespace IntegrationTests.Features.Friendships
{
    public class GetFriendListQueryTest : BaseIntegrationTest
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _client;
        private readonly Guid userId = Guid.NewGuid();
        private readonly int INITIATED_FRIENDSHIPS_NUMBER = 3;
        private readonly int RECEIVED_FRIENDSHIP_NUMBERS = 7;
        private readonly int PAGE_NUMBER = 1;
        private readonly int PAGE_SIZE = 10;
        public GetFriendListQueryTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _client = factory.CreateClient();
            _context = factory._dbContext;
        }

        private async Task<List<Friendship>> GetSeedFriendships()
        {
            var userList = new List<ApplicationUser>();
            for(var i = 0; i < INITIATED_FRIENDSHIPS_NUMBER + RECEIVED_FRIENDSHIP_NUMBERS; i++)
            {
                var userId = Guid.NewGuid();
                var user = new ApplicationUser(userId, "Nguyen", $"User {i}");
                userList.Add(user);
            }
            userList.Add(new ApplicationUser(userId, "Nguyen", "Hong Quan"));

            await _context.AddRangeAsync(userList);
            await _context.SaveChangesAsync();

            var friendships = new List<Friendship>();
            var userIndex = 0;
            for(var i = 0; i < INITIATED_FRIENDSHIPS_NUMBER; i++)
            {
                friendships.Add(new Friendship(
                    Guid.NewGuid(),
                    userId.ToString(),
                    userList[userIndex++].Id
                ));
            }

            for(var i = 0; i < RECEIVED_FRIENDSHIP_NUMBERS; i++)
            {
                friendships.Add(new Friendship(
                    Guid.NewGuid(),
                    userList[userIndex++].Id,
                    userId.ToString()
                ));
            }

            await _context.Friendships.AddRangeAsync(friendships);
            await _context.SaveChangesAsync();

            return friendships;
        } 

        [Theory]
        [InlineData(FriendshipFilter.Initiated)]
        [InlineData(FriendshipFilter.Received)]
        [InlineData(FriendshipFilter.Both)]
        public async Task GetFriendList_ShouldReturnCorrectPaginationResponse(FriendshipFilter filter)
        {
            var friendships = await GetSeedFriendships();
            var response = new HttpResponseMessage();
            switch(filter)
            {
                case FriendshipFilter.Initiated:
                    response = await _client.GetAsync($"/api/Friendship/initiated?userId={userId}&status={FriendshipStatus.Pending}&pageNumber={PAGE_NUMBER}&pageSize={PAGE_SIZE}");
                break;

                case FriendshipFilter.Received:
                    response = await _client.GetAsync($"/api/Friendship/received?userId={userId}&status={FriendshipStatus.Pending}&pageNumber={PAGE_NUMBER}&pageSize={PAGE_SIZE}");
                break;

                case FriendshipFilter.Both:
                    response = await _client.GetAsync($"/api/Friendship/friendlist?userId={userId}&status={FriendshipStatus.Pending}&pageNumber={PAGE_NUMBER}&pageSize={PAGE_SIZE}");
                break;
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<PagedResponse<List<FriendshipDto>>>();
            
            content.Should().NotBeNull();
            var numberOfRecords = 0;
            switch(filter)
            {
                case FriendshipFilter.Initiated:
                    numberOfRecords = INITIATED_FRIENDSHIPS_NUMBER;
                    break;

                case FriendshipFilter.Received:
                    numberOfRecords = RECEIVED_FRIENDSHIP_NUMBERS;
                    break;

                case FriendshipFilter.Both:
                    numberOfRecords = RECEIVED_FRIENDSHIP_NUMBERS + INITIATED_FRIENDSHIPS_NUMBER;
                    break;
            }
            content.TotalPages.Should().Be((int)Math.Ceiling(numberOfRecords / (double)PAGE_SIZE));
            content.TotalRecords.Should().Be(numberOfRecords);
            content.PageNumber.Should().Be(PAGE_NUMBER);
            content.PageSize.Should().Be(PAGE_SIZE);
        }

    }
}