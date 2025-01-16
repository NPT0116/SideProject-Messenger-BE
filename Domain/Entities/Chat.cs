using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Chat
    {
        public Chat(
            Guid id,
            ChatType type)
        {
            TopicEmoji = "👍";
            Type = type;
            Id = id;
        }
        public Guid Id { get; private set; }
        public string TopicEmoji { get; private set; }
        public ChatType Type { get; private set; }

        // Navigation Property
        public IReadOnlyCollection<Participant> Participants { get; private set; }
        public IReadOnlyCollection<Message> Messages { get; private set; }
    }
}
