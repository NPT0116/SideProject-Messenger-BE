using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Dtos.Friendships
{
    public class CreateFriendshipDto
    {
        
        public Guid InitiatorId { get; private set; }
        public Guid ReceiverId { get; private set; }
    }
}