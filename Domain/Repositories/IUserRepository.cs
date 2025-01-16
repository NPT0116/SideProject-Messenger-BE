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
        public List<User> GetUsers();
        public User GetUserById(Guid id);
        public Task<User> CreateUserAsync(User user);
        public Task<User> UpdateUserAsync(User user);
        public void DeleteUser(Guid id);
    }
}
