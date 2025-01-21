using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ApplicationDbContext _context;

        public ParticipantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Participant> CreateAsync(Participant participant)
        {
            // Ensure the user exists in the Users table
            var userExists = await _context.Users.AnyAsync(u => u.Id == participant.UserId);
            if (!userExists)
            {
                throw new Exception("User does not exist in the Users table.");
            }

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();
            return participant;
        }

        public async Task<Participant> DeleteAsync(Guid id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null)
            {
                return null;
            }

            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();
            return participant;
        }

        public async Task<Participant> GetByIdAsync(Guid id)
        {
            return await _context.Participants
                .Include(p => p.Chat)
                .Include(p => p.Messages)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<Guid> GetParticipantIdByUserIdAndChatIdAsync(Guid userId, Guid chatId)
        {
            var userIdString = userId.ToString();
            return _context.Participants
                .Where(p => p.UserId == userIdString && p.ChatId == chatId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Participant>> GetParticipantsByChatIdAsync(Guid chatId)
        {
            return await _context.Participants
                .Where(p => p.ChatId == chatId)
                .ToListAsync();
        }

        public async Task<Participant> UpdateAsync(Participant participant)
        {
            _context.Participants.Update(participant);
            await _context.SaveChangesAsync();
            return participant;
        }
    }
}