// filepath: /C:/Users/Admin/Desktop/web_messenger/Application/Validators/RegisterUserRequestValidator.cs
using FluentValidation;
using Application.Dtos.Users;
using Application.Features.Auth.Register;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.request.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.request.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.request.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}