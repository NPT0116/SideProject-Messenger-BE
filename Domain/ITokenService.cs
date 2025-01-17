using System;
using Domain.Entities;

namespace Domain;

public interface ITokenService
{
    string CreateUserToken(User user);

    Guid? GetUserIdFromToken(string token);
}
