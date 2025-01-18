using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IFriendshipRepository
    {
        Task<List<User>> GetFriendList(Guid userId);
        Task CreateFriendship(Guid initiatorId, Guid receiverId);
    }
}