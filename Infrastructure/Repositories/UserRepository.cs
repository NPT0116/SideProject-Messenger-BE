// Infrastructure/Repositories/UserRepository.cs
using Application.Dtos.Users;
using Domain;
using Domain.Entities;
using Domain.Exceptions.Users;
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

        public async Task<bool> checkPasswordAsync(User user, string password)
        {
            var applicationUser = UserMapper.ToApplicationUser(user);
            return await _userManager.CheckPasswordAsync(applicationUser, password);
        }

        public async Task<User> CreateUserAsync(User _user)
    {
        var user = new ApplicationUser(
            
            Guid.NewGuid(),
            _user.FirstName,
            _user.LastName        
        )
        {
            UserName = _user.UserName
        };
        Console.WriteLine($"User: {user.FirstName}, {user.LastName}");
        var result = await _userManager.CreateAsync(user, _user.PasswordHash);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ApplicationException($"Failed to create user: {errors}");
        }

        return UserMapper.ToDomainUser(user);
    }

        public void DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public User GetUserById(Guid id)
        {
            var user = _userManager.FindByIdAsync(id.ToString()).Result;
            return user == null ?   null: UserMapper.ToDomainUser(user);
        }

        public async Task<User> GetUserByUsernameAsync(string UserName)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            return user == null ? null : UserMapper.ToDomainUser(user);
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