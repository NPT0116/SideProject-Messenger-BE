using System;
using System.Net;

namespace Domain.Exceptions.BadRequest;

public class BadRequestCustomException : BaseException
{
    public BadRequestCustomException(string message): base(message,HttpStatusCode.BadRequest)
    {
        
    }
}
