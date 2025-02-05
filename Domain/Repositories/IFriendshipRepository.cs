using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Dtos.Friendship;
using Domain.Dtos.Shared;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Repositories
{
    public interface IFriendshipRepository
    {
        Task<PageResponseDto<FriendshipListResponseDto>> GetInitiatedFriendList(Guid userId, FriendshipStatus? status, int pageNumber, int pageSize);
        Task<PageResponseDto<FriendshipListResponseDto>> GetReceivedFriendList(Guid userId, FriendshipStatus? status, int pageNumber, int pageSize);
        Task<PageResponseDto<FriendshipListResponseDto>> GetFriendList(Guid userId, FriendshipStatus? status, int pageNumber, int pageSize);
        Task UpdateFriendshipStatusById(Guid friendshipId, FriendshipStatus status);
        Task<Friendship?> GetFriendshipById(Guid friendshipId);
        Task CreateFriendship(Guid initiatorId, Guid receiverId);
        Task<Friendship?> GetFriendshipBetweenTwoUsersByIds(Guid initiatorId, Guid receiverId);
    }
}