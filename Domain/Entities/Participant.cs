using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Participant
    {
        public Participant(
            Guid id,
            string userId,
            Guid chatId)
        {
            Id = id;
            UserId = userId;
            ChatId = chatId;
        }
        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public Guid ChatId { get; private set; }
        public string? NickName { get; private set; }

        // Navigation Property
        public Chat Chat { get; private set; }
        public IReadOnlyCollection<Message> Messages { get; private set; }

    }
}
