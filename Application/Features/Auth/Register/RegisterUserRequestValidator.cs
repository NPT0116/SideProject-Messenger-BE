// filepath: /C:/Users/Admin/Desktop/web_messenger/Application/Validators/RegisterUserRequestValidator.cs
using FluentValidation;
using Application.Dtos.Users;
using Application.Features.Auth.Register;
using Domain.Repositories;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserRequestValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.request.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.request.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.request.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.request.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MustAsync(async (username, cancellation) => !await UserNameExists(username))
            .WithMessage("Username already exists.");
    }

    private async Task<bool> UserNameExists(string username)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        return user != null;
    }
}