using System;
using Domain.Entities;

namespace Domain.Repositories;

public interface IMessageRepository
{
    public Task<Message> GetByIdAsync(Guid id);
    public Task<Message> CreateAsync(Message message);
    public Task<Message> UpdateAsync(Message message);
    public Task<Message> DeleteAsync(Guid id);
    public Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId);

}
