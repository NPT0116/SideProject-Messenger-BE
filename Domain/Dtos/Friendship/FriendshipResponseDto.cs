using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Dtos.Friendship
{
    public class FriendshipResponseDto
    {
        public FriendshipResponseDto(Entities.Friendship friendship, User initiator)
        {
            Friendship = friendship;
            Initiator = initiator;
        }
        public Entities.Friendship Friendship { get; set; }
        public User Initiator { get; set; }
    }
}