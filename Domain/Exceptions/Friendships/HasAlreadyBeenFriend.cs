using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Domain.Exceptions.Friendships
{
    public class HasAlreadyBeenFriend : Exception
    {
        public HasAlreadyBeenFriend(Guid initiatorId, Guid receiverId)
            : base($"User with ID {initiatorId} is already friends with user with ID {receiverId}.")
        {
        }

        public HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    }
}