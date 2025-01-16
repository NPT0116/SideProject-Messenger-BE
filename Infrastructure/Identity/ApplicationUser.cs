using System;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationUser: IdentityUser
{
          public ApplicationUser(
            Guid id, 
            string firstName,
            string lastName,
            string passwordHash)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
        }
        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime LastSeen { get; private set; }
        public bool IsOnline { get; private set; }
        public Guid ProfilePictureId { get; private set; }
}
