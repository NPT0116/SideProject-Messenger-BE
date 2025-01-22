using System;
using Domain.Entities;

namespace Domain.Repositories;

public interface IParticipantRepository
{
    Task<Participant> CreateAsync(Participant participant);
    Task<Participant> DeleteAsync(Guid id);
    Task<Participant> GetByIdAsync(Guid id);
    Task<IEnumerable<Participant>> GetParticipantsByChatIdAsync(Guid chatId);
    Task<Participant> UpdateAsync(Participant participant);

    Task<Guid> GetParticipantIdByUserIdAndChatIdAsync(Guid userId, Guid chatId);

}
