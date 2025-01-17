using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Friendship
    {
        public Friendship(
            Guid id,
            Guid initiatorId,
            Guid receiverId)
        {
            Id = id;
            InitiatorId = initiatorId;
            ReceiverId = receiverId;
            Status = FriendshipStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }
        public Guid Id { get; private set; }
        public Guid InitiatorId {  get; private set; }
        public Guid ReceiverId { get; private set; }
        public FriendshipStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Navigation Property
            public User Initiator { get; private set; } = null!;
        public User Receiver { get; private set; } = null!;
    }
}
