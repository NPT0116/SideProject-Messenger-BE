using System;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> CreateAsync(Chat chat)
        {
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<Chat> DeleteAsync(Guid id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat == null)
            {
                return null;
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<Chat> GetByIdAsync(Guid id)
        {
            return await _context.Chats
                .Include(c => c.Participants)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId)
        {
            return await _context.Chats
                .Include(c => c.Participants)
                .Where(c => c.Participants.Any(p => Guid.Parse(p.UserId) == userId))
                .ToListAsync();
        }

    public async Task<bool> IsUserInChatAsync(Guid chatId, Guid userId)
    {
        Console.WriteLine($"ChatId: {chatId}" + $"UserId: {userId}");
        var UserInChat = await _context.Participants.AnyAsync(p => p.ChatId == chatId && p.UserId == userId.ToString());
        Console.WriteLine($"UserInChat: {UserInChat}");
        return UserInChat;
    }

    public async Task<Chat> UpdateAsync(Chat chat)
        {
            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();
            return chat;
        }
      public async Task<Chat> GetChatBetweenUsersAsync(Guid user1Id, Guid user2Id)
        {
            var user1IdStr = user1Id.ToString();
            var user2IdStr = user2Id.ToString();

            return await _context.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Participants.Any(p => p.UserId == user1IdStr) &&
                                          c.Participants.Any(p => p.UserId == user2IdStr));
        }

        
        public async Task<List<string>> GetUsersInChatAsync(Guid chatId)
        {
            var participants = await _context.Participants
                .Where(p => p.ChatId == chatId)
                .Select(p => p.UserId)
                .ToListAsync();

            Console.WriteLine($"Users in ChatId: {chatId}");
            foreach (var userId in participants)
            {
                Console.WriteLine($"UserId: {userId}");
            }

            return participants;
        }
    }