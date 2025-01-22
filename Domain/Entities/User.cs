// filepath: /c:/Users/Admin/Desktop/web_messenger/Domain/Entities/User.cs
namespace Domain.Entities
{
    public class User
    {
        public User() { }

        public User(Guid id,string username, string firstName, string lastName, string passwordHash)
        {
            Id = id;
            UserName = username;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
        }
        public User(string username, string firstName, string lastName, string passwordHash)
        {   
            UserName = username;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
        }

        public void SetId(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
        public string UserName { get; set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime LastSeen { get; set; }
        public bool IsOnline { get; set; }
        public Guid ProfilePictureId { get; set; }
        public Attachment? ProfilePicture { get; private set; }
    }
}