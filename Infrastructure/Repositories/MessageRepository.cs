using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Message> CreateAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Message> DeleteAsync(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return null;
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return message;
        }



        public async Task<Message> GetByIdAsync(Guid id)
        {
            return await _context.Messages
                .Include(m => m.Chat)
                .Include(m => m.Sender)
                .Include(m => m.Attachment)
                .Include(m => m.Reactions)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId)
        {
            return await _context.Messages
                .Include(m => m.Chat)
                .Include(m => m.Sender)
                .Include(m => m.Attachment)
                .Include(m => m.Reactions)
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<Message> UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            return message;
        }
    }
}