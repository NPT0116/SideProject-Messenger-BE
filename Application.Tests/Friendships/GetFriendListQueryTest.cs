using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Friendship.GetFriendList;
using Domain.Dtos.Friendship;
using Domain.Dtos.Shared;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Moq;

namespace Application.Tests.Friendships
{
    public class GetFriendListQueryTest
    {
        private readonly Mock<IFriendshipRepository> _friendshipRepositoryMock;
        private readonly GetFriendListQueryHandler _handler;
        public GetFriendListQueryTest()
        {
            _friendshipRepositoryMock = new Mock<IFriendshipRepository>();
            _handler = new GetFriendListQueryHandler(_friendshipRepositoryMock.Object);
        }

        private static PageResponseDto<FriendshipListResponseDto> GetMockFriendList(Guid user1Id, Guid user2Id)
        {
            var friendships = new List<FriendshipListResponseDto>();
            var initiator = new User(user1Id, "InitiatorUser", "John", "Doe", Guid.NewGuid().ToString());
            var receiver = new User(user2Id, "ReceiverUser", "Jane", "Smith", Guid.NewGuid().ToString());

            // 3 Initiated Friendships
            for (int i = 0; i < 3; i++)
            {
                friendships.Add(new FriendshipListResponseDto(
                    new Friendship(Guid.NewGuid(), initiator.Id.ToString(), receiver.Id.ToString()),
                    initiator, // User is the initiator
                    receiver
                ));
            }

            // 7 Received Friendships
            for (int i = 0; i < 7; i++)
            {
                friendships.Add(new FriendshipListResponseDto(
                    new Friendship(Guid.NewGuid(), receiver.Id.ToString(), initiator.Id.ToString()),
                    receiver, // User is the initiator
                    initiator
                ));
            }

            return new PageResponseDto<FriendshipListResponseDto>
            {
                Data = friendships,
                PageNumber = 1,
                PageSize = 10,
                TotalRecords = 10,
                TotalPages = 1
            };
        }

        [Theory]
        [InlineData(FriendshipFilter.Initiated, 3)]
        [InlineData(FriendshipFilter.Received, 7)]
        [InlineData(FriendshipFilter.Both, 10)]
        public async Task Handle_ShouldReturnCorrectFriendListBasedOnFilter(FriendshipFilter filter, int expectedCount)
        {
            // Arrange
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();
            var status = FriendshipStatus.Approved;
            var pageNumber = 1;
            var pageSize = 10;
            var request = new GetFriendListQuery(user1Id, status, filter, pageNumber, pageSize);

            var mockData = GetMockFriendList(user1Id, user2Id);

            // Instead of hardcoding the return value, dynamically filter the mock data
            switch(filter)
            {
                case FriendshipFilter.Initiated:
                    _friendshipRepositoryMock.Setup(repo => repo.GetInitiatedFriendList(user1Id, status, pageNumber, pageSize))
                        .ReturnsAsync(() =>
                        {
                            var filteredData = mockData.Data.Where(f => f.Initiator.Id == user1Id).ToList();
                            return new PageResponseDto<FriendshipListResponseDto>
                            {
                                Data = filteredData,
                                PageNumber = 1,
                                PageSize = 10,
                                TotalRecords = filteredData.Count,
                                TotalPages = 1
                            };
                        });
                    break;
                case FriendshipFilter.Received:
                    _friendshipRepositoryMock.Setup(repo => repo.GetReceivedFriendList(user1Id, status, pageNumber, pageSize))
                        .ReturnsAsync(() =>
                        {
                            var filteredData = mockData.Data.Where(f => f.Receiver.Id == user1Id).ToList();
                            return new PageResponseDto<FriendshipListResponseDto>
                            {
                                Data = filteredData,
                                PageNumber = 1,
                                PageSize = 10,
                                TotalRecords = filteredData.Count,
                                TotalPages = 1
                            };
                        });
                        break;
                case FriendshipFilter.Both:
                    _friendshipRepositoryMock.Setup(repo => repo.GetFriendList(user1Id, status, pageNumber, pageSize))
                        .ReturnsAsync(() => mockData);
                    break;
            }



            // Act: Call the actual function
            var result = (await _handler.Handle(request, CancellationToken.None)).response;

            // Extract actual result count
            var actualCount = result.TotalRecords;

            // Assert: Validate correctness
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(expectedCount, actualCount);

            // Verify correct repository method was called
            switch (filter)
            {
                case FriendshipFilter.Initiated:
                    _friendshipRepositoryMock.Verify(repo => repo.GetInitiatedFriendList(user1Id, status, pageNumber, pageSize), Times.Once);
                    break;
                case FriendshipFilter.Received:
                    _friendshipRepositoryMock.Verify(repo => repo.GetReceivedFriendList(user1Id, status, pageNumber, pageSize), Times.Once);
                    break;
                case FriendshipFilter.Both:
                    _friendshipRepositoryMock.Verify(repo => repo.GetFriendList(user1Id, status, pageNumber, pageSize), Times.Once);
                    break;
            }
        }

        
    }
}