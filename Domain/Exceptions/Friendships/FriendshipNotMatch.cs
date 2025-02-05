using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Domain.Exceptions.Friendships
{
    public class FriendshipNotMatch : BaseException
    {
        public FriendshipNotMatch(Guid friendshipId, string userId)
            : base($"Friendship {friendshipId} is not sent to user {userId}.",
             HttpStatusCode.Conflict)
        {
        }
    }
}