using System;
using System.Net;

namespace Domain.Exceptions.Users;

public class FailCreateUser : BaseException
{
    public FailCreateUser(string name) : base($"Fail to create user: {name}", HttpStatusCode.InternalServerError)
    {

    }
}
