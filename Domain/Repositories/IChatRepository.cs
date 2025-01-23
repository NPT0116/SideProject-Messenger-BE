using System;
using Domain.Entities;

namespace Domain.Repositories;

public interface IChatRepository
{
    public Task<Chat> CreateAsync(Chat chat);
    public Task<Chat> GetByIdAsync(Guid id);
    public Task<Chat> UpdateAsync(Chat chat);
    public Task<Chat> DeleteAsync(Guid id);
    
    public Task<bool> IsUserInChatAsync(Guid chatId, Guid userId);
    public Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId);
    public Task<Chat> GetChatBetweenUsersAsync(Guid user1Id, Guid user2Id);
    public Task<List<string>> GetUsersInChatAsync(Guid chatId);

}
