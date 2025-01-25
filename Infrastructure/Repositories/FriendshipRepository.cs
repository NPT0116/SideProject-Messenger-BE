using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Friendship.GetFriendList;
using Domain.Dtos.Friendship;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions.Friendships;
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

        public async Task<List<FriendshipResponseDto>> GetFriendList(Guid userId, FriendshipStatus? status)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if(user == null)
            {
                throw new UserNotFound(userId); 
            }        

            var friendsInitiated = await _context.Friendships
                .Where(f => f.InitiatorId == userId.ToString() && (status == null || f.Status == status))
                .Select(f => new
                {
                    Friendship = f,
                    User = UserMapper.ToApplicationUser(user)
                }).ToListAsync();
                // .Join(
                //     _context.Users,
                //     friendship => friendship.ReceiverId,
                //     user => user.Id,
                //     (friendship) => new
                //     {
                //         Friendship = friendship,
                //         User = user
                //     });

            var friendsReceived = await _context.Friendships
                .Where(f => f.ReceiverId == userId.ToString() && (status == null || f.Status == status))
                .Join(
                    _context.Users,
                    friendship => friendship.InitiatorId,
                    user => user.Id,
                    (friendship, user) => new
                    {
                        Friendship = friendship,
                        User = user
                    })
                .ToListAsync();

            var friends = friendsInitiated.Concat(friendsReceived);
            var friendshipList = friends.Select(fr => new
            {
                Friendship = fr.Friendship,
                User = UserMapper.ToDomainUser(fr.User)
            }).ToList();

            return friendshipList.Select(friend => new FriendshipResponseDto(friend.Friendship, friend.User)).ToList();
        }

        public async Task<Friendship?> GetFriendshipBetweenTwoUsersByIds(Guid initiatorId, Guid receiverId)
        {
            return await _context.Friendships.FirstOrDefaultAsync(fr =>
                (fr.InitiatorId == initiatorId.ToString() && fr.ReceiverId == receiverId.ToString()) ||
                (fr.InitiatorId == receiverId.ToString() && fr.ReceiverId == initiatorId.ToString())
            );
        }


        public async Task<Friendship?> GetFriendshipById(Guid friendshipId)
        {
            return await _context.Friendships.FindAsync(friendshipId);
        }


        public async Task UpdateFriendshipStatusById(Guid friendshipId, FriendshipStatus status)
        {
            var friendship = await _context.Friendships.FindAsync(friendshipId);
            if(friendship == null)
            {
                throw new FriendshipNotFound(friendshipId);
            }

            friendship.SetFriendshipStatus(status);
            await _context.SaveChangesAsync();
        }

    }
}