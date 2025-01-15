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
                new User(new Guid(), "Nguyen"," Hong Quan", "saddasdas"),
                new User(new Guid(), "Nguyen","Phuc Thanh", "sadasdsds")
            };
        }
    }
}
