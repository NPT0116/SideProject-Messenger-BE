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
            var friendship = new Friendship(friendshipId, initiatorId.ToString(), receiverId.ToString());
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
                .Where(f => f.InitiatorId == userId.ToString())
                .Join(
                    _context.Users,
                    friendship => friendship.ReceiverId,
                    user => user.Id,
                    (friendship, user) => user);

            var friendsReceived = _context.Friendships
                .Where(f => f.ReceiverId == userId.ToString())
                .Join(
                    _context.Users,
                    friendship => friendship.InitiatorId,
                    user => user.Id,
                    (friendship, user) => user);


            var friends = await friendsInitiated.Union(friendsReceived).ToListAsync();

            return friends.Select(friend => new User(
                Guid.Parse(friend.Id),
                friend.UserName,
                friend.FirstName,
                friend.LastName,
                friend.PasswordHash)).ToList();
        }
    }
}