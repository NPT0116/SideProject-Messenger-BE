using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Exceptions.Users;
using Domain.Repositories;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly IUserRepository _userRepository;
        public FriendshipRepository(
            ApplicationDbContext context,
            IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task CreateFriendship(Guid initiatorId, Guid receiverId)
        {
            var friendshipId = Guid.NewGuid();
            var friendship = new Friendship(friendshipId, initiatorId, receiverId);
            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetFriendList(Guid userId)
        {
            var user = _userRepository.GetUserById(userId);
            if(user == null)
            {
                throw new UserNotFound(userId); 
            }        

            var friendsInitiated = _context.Friendships
                .Where(f => f.InitiatorId == userId)
                .Join(
                    _context.Users,
                    friendship => friendship.ReceiverId,
                    user => UserMapper.ToDomainUser(user).Id,
                    (friendship, user) => UserMapper.ToDomainUser(user));

            var friendsReceived = _context.Friendships
                .Where(f => f.ReceiverId == userId)
                .Join(
                    _context.Users,
                    friendship => friendship.InitiatorId,
                    user => UserMapper.ToDomainUser(user).Id,
                    (friendship, user) => UserMapper.ToDomainUser(user));


            var friends = await friendsInitiated.Union(friendsReceived).ToListAsync();

            return friends;
        }
    }
}