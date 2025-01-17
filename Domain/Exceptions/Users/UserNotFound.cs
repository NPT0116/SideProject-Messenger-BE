using System;
using System.Net;

namespace Domain.Exceptions.Users;

public class UserNotFound : BaseException
{
    public UserNotFound(Guid id)
        : base($"User with id {id} not found", HttpStatusCode.NotFound)
    {
    }

        public UserNotFound(string UserName)
        : base($"User with UserName:  {UserName} not found", HttpStatusCode.NotFound)
    {
    }
}
