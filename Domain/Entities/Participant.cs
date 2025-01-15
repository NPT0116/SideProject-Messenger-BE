using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Participant
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid ChatId { get; private set; }
        public string NickName { get; private set; }

        // Navigation Property
        public User User { get; private set; }
        public Chat Chat { get; private set; }
        public IReadOnlyCollection<Message> Messages { get; private set; }

    }
}
