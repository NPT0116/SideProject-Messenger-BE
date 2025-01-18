using System;

namespace Infrastructure.Identity;

using Domain.Entities;
using Infrastructure.Identity;

public static class UserMapper
{
    public static User ToDomainUser(ApplicationUser applicationUser)
{
    if (applicationUser == null) return null;

    var domainUser = new User(
        Guid.Parse(applicationUser.Id),
        applicationUser.UserName,
        applicationUser.FirstName,
        applicationUser.LastName,
        applicationUser.PasswordHash)
    {
        LastSeen = applicationUser.LastSeen,
        IsOnline = applicationUser.IsOnline,
        ProfilePictureId = applicationUser.ProfilePictureId
    };
    return domainUser;
}

    public static ApplicationUser ToApplicationUser(User domainUser)
{
    if (domainUser == null) return null;

    return new ApplicationUser(
        domainUser.Id,
        domainUser.FirstName, 
        domainUser.LastName
        )
    {
        PasswordHash = domainUser.PasswordHash,
        UserName = domainUser.UserName,
        LastSeen = domainUser.LastSeen,
        IsOnline = domainUser.IsOnline,
        ProfilePictureId = domainUser.ProfilePictureId
    };
}

}

