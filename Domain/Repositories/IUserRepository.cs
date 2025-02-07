using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsersAsync();
        public Task<User?> GetUserByIdAsync(Guid id);
        public Task<User> GetUserByUsernameAsync(string UserName);
        public Task<User> CreateUserAsync(User user);
        public Task<User> UpdateUserAsync(User user);
        public void DeleteUser(Guid id);
        public  Task<bool> ExistsAsync(Guid userId);

        public Task<User> GetUserFromParticipantId(Guid participantId);
        public Task<bool> checkPasswordAsync (User user, string password);
    }
}
