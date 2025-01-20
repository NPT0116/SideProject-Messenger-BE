using Application.Dtos.Users;
using Domain;
using Domain.Entities;
using Domain.Exceptions.Users;
using Domain.Repositories;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    internal class UserRepository : IUserRepository
    {
   private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
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

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            string idAsString = id.ToString();
            var userEntity = await _userManager.Users
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(u => u.Id == idAsString);

            return userEntity == null ? null : UserMapper.ToDomainUser(userEntity);
        }

        public async Task<User> GetUserByUsernameAsync(string UserName)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            return user == null ? null : UserMapper.ToDomainUser(user);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _userManager.Users
                .Select(u => UserMapper.ToDomainUser(u))
                .ToListAsync();
        }


     
        public async Task<User> UpdateUserAsync(User user)
        {
            var applicationUser = UserMapper.ToApplicationUser(user);
            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == applicationUser.Id);

            if (existingUser != null)
            {
                _context.Entry(existingUser).State = EntityState.Detached;
            }

            _context.Users.Attach(applicationUser);
            _context.Entry(applicationUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (ApplicationUser)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();

                if (databaseEntry == null)
                {
                    throw new Exception("Unable to save changes. The user was deleted by another user.");
                }
                else
                {
                    var databaseValues = (ApplicationUser)databaseEntry.ToObject();

                    // Update the original values with the database values
                    entry.OriginalValues.SetValues(databaseValues);

                    // Retry the update operation
                    await _context.SaveChangesAsync();
                }
            }

            return UserMapper.ToDomainUser(applicationUser);
        }



    }
}