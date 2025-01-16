using Microsoft.AspNetCore.Identity;
using System;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() { }

        public ApplicationUser(Guid id, string firstName, string lastName, string passwordHash)
        {
            Id = id.ToString();
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime LastSeen { get;  set; }
        public bool IsOnline { get;  set; }
        public Guid ProfilePictureId { get;  set; }
    }
}