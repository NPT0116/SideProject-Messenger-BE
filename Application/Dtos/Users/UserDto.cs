using System;

namespace Application.Dtos.Users;

public class RegisterUserResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public record RegisterUserRequestDto {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
}