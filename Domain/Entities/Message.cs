﻿using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Message
    {
        public Message()
        {
            // Required by EF
        }
        public Message(
            Guid id,
            string content,
            MessageType type,
            Guid senderId,
            Guid chatId)
        {
            Id = id;
            Content = content;
            Type = type;
            SenderId = senderId;
            ChatId = chatId;
            SentAt = DateTime.UtcNow;
            AttachmentId = null;
        }
        public Guid Id { get; private set; }
        public string Content { get; private set; }
        public MessageType Type { get; private set; }
        public DateTime SentAt { get; private set; }
        public bool IsRead { get; private set; }
        public bool IsUpdated { get; private set; }
        public bool IsDeleted { get; private set; }
        public Guid SenderId { get; private set; }
        public Guid ChatId { get; private set; }
        public Guid? AttachmentId { get; private set; }

        // Navigation Property
        public Chat Chat { get; private set; }
        public Participant Sender { get; private set; }
        public Attachment? Attachment { get; private set; }
        public IReadOnlyCollection<Reaction> Reactions { get; private set; }
    }
}


