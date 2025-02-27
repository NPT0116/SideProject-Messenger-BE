﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Reaction
    {
        public Reaction(
            Guid id,
            string content,
            Guid messageId)
        {
            Id = id; 
            Content = content; 
            MessageId = messageId;
        }
        public Guid Id { get; private set; }
        public string Content { get; private set; }
        public Guid MessageId { get; private set; }

        // Navigation Property
        public Message Message { get; private set; }
    }
}

// Use case 