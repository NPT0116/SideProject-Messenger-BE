using System;
using System.Net;

namespace Domain.Exceptions.File;

public class EmptyFile: BaseException
{
    public EmptyFile() : base("File is empty", HttpStatusCode.BadRequest)
    {
    }
}
