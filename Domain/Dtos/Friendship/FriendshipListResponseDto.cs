using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Dtos.Friendship
{
    public class FriendshipListResponseDto
    {
        public FriendshipListResponseDto(
            Entities.Friendship friendship, 
            User initiator, 
            User receiver)
        {
            Friendship = friendship;
            Initiator = initiator;
            Receiver = receiver;

        }
        public Entities.Friendship Friendship { get; set; }
        public User Initiator { get; set; }
        public User Receiver { get; set; }
    }
}