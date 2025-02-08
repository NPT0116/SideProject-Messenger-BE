using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Friendship.GetFriendList;
using Domain.Dtos.Friendship;
using Domain.Dtos.Shared;
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

        public async Task<Guid> CreateFriendship(Guid initiatorId, Guid receiverId)
        {
            var friendshipId = Guid.NewGuid();
            var friendship = new Friendship(friendshipId, initiatorId.ToString(), receiverId.ToString());
            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();
            return friendshipId;
        }

        public async Task<PageResponseDto<FriendshipListResponseDto>> GetFriendList(Guid userId, FriendshipStatus? status, int pageNumber, int pageSize)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if(user == null)
            {
                throw new UserNotFound(userId); 
            }     

            var initiatedFriendshipsQuery = _context.Friendships
                .Where(f => f.InitiatorId == userId.ToString() && (status == null || f.Status == status))
                .Select(f => new
                {
                    f,
                    ReceiverId = f.ReceiverId,
                    InitiatorId = f.InitiatorId
                });

            var receivedFriendshipsQuery = _context.Friendships
                .Where(f => f.ReceiverId == userId.ToString() && (status == null || f.Status == status))
                .Select(f => new
                {
                    f,
                    ReceiverId = f.ReceiverId,
                    InitiatorId = f.InitiatorId
                });

            // ✅ Perform Union before applying transformations
            var friendshipsQuery = initiatedFriendshipsQuery.Union(receivedFriendshipsQuery).AsQueryable();
            
            // 🔹 Apply pagination BEFORE fetching data
            var totalRecords = await friendshipsQuery.CountAsync();
            var friendships = await friendshipsQuery
                .OrderBy(f => f.f.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ✅ Apply `UserMapper.ToDomainUser` AFTER fetching data
            var transformedFriendships = friendships.Select(friend => new FriendshipListResponseDto
            (
                friend.f,
                UserMapper.ToDomainUser(_context.Users.FirstOrDefault(u => u.Id == friend.InitiatorId)),
                UserMapper.ToDomainUser(_context.Users.FirstOrDefault(u => u.Id == friend.ReceiverId))
            )).ToList();

            // ✅ Return paginated response
            return new PageResponseDto<FriendshipListResponseDto>
            {
                Data = transformedFriendships,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };

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

        public async Task<PageResponseDto<FriendshipListResponseDto>> GetInitiatedFriendList(Guid userId, FriendshipStatus? status, int pageNumber, int pageSize)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if(user == null)
            {
                throw new UserNotFound(userId); 
            }

            var applicationUser = UserMapper.ToApplicationUser(user);

            var friendsInitiated = _context.Friendships
                .Where(f => f.InitiatorId == userId.ToString() && (status == null || f.Status == status))
                .Join(_context.Users,
                    friendship => friendship.ReceiverId,
                    user => user.Id,
                    (friendship, user) => new
                    {
                        Friendship = friendship,
                        User = user
                    })
                .Select(f => new
                {
                    f.Friendship,
                    Initiator = applicationUser,
                    Receiver = f.User
                }).AsQueryable();

            var totalRecords = await friendsInitiated.CountAsync();

            var friendships = await friendsInitiated
                .OrderBy(f => f.Friendship.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResponseDto<FriendshipListResponseDto>
            {
                Data = friendships.Select(friend => new FriendshipListResponseDto(
                    friend.Friendship, 
                    UserMapper.ToDomainUser(friend.Initiator),
                    UserMapper.ToDomainUser(friend.Receiver))).ToList(),
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }

        public async Task<PageResponseDto<FriendshipListResponseDto>> GetReceivedFriendList(Guid userId, FriendshipStatus? status, int pageNumber, int pageSize)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if(user == null)
            {
                throw new UserNotFound(userId); 
            }

            var friendsReceived = _context.Friendships
                .Where(f => f.ReceiverId == userId.ToString() && (status == null || f.Status == status))
                .Join(
                    _context.Users,
                    friendship => friendship.InitiatorId,
                    user => user.Id,
                    (friendship, initiator) => new
                    {
                        Friendship = friendship,
                        Receiver = user,
                        Initiator = initiator
                    })
                .AsQueryable();

            var totalRecords = await friendsReceived.CountAsync();
            var friendships = await friendsReceived.ToListAsync();

            return new PageResponseDto<FriendshipListResponseDto>
            {
                Data = friendships.Select(friend => new FriendshipListResponseDto(
                    friend.Friendship, 
                    UserMapper.ToDomainUser(friend.Initiator),
                    friend.Receiver)).ToList(),
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
               
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