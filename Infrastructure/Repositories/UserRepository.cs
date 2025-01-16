// Infrastructure/Repositories/UserRepository.cs
using Application.Dtos.Users;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

public async Task<User> CreateUserAsync(User _user)
{
    var user = new ApplicationUser(
        Guid.NewGuid(),
        _user.FirstName,
        _user.LastName,
        null // PasswordHash sẽ được ASP.NET Identity xử lý
    );

    var result = await _userManager.CreateAsync(user, user.PasswordHash);
    if (!result.Succeeded)
    {
        throw new ApplicationException("Failed to create user");
    }

    return UserMapper.ToDomainUser(user);
}


        public void DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public User GetUserById(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<User> GetUsers()
        {
            return _userManager.Users
                .Select(u => UserMapper.ToDomainUser(u))
                .ToList();
        }

        public Task<User> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }


    }
}