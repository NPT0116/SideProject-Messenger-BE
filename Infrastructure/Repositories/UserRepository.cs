using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Repositories
{
    internal class UserRepository : IUserRepository
    {
        public List<User> GetUsers()
        {
            return new List<User>
            {
                new User(1, "Nguyen Hong Quan"),
                new User(2, "Nguyen Phuc Thanh")
            };
        }
    }
}
