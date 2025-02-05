using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Domain.Exceptions.Friendships
{
    public class FriendshipNotFound : BaseException
    {
        public FriendshipNotFound(Guid friendshipId)
            : base($"Friendship {friendshipId} has not been intialized.",
             HttpStatusCode.NotFound)
        {
        }
    }
}