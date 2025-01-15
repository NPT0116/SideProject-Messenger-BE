using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public User(
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
        public Attachment? ProfilePicture { get; private set; }
        public ICollection<Friendship> FriendshipsInitiated { get; private set; } = new List<Friendship>();
        public ICollection<Friendship> FriendshipsReceived { get; private set; } = new List<Friendship>();
        public IReadOnlyCollection<Participant> Participants { get; private set; }
    }
}
