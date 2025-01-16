using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<User> GetUsers()
        {
            var users = _context.Users.ToList();
            return users;
        }
    }
}
