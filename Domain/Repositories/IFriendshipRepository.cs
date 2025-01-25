using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Repositories
{
    public interface IFriendshipRepository
    {
        Task<List<User>> GetFriendList(Guid userId, FriendshipStatus? status);
        Task UpdateFriendshipStatusById(Guid friendshipId, FriendshipStatus status);
        Task<Friendship?> GetFriendshipById(Guid friendshipId);
        Task CreateFriendship(Guid initiatorId, Guid receiverId);
    }
}