using System;
using FluentValidation;

namespace Application.Features.Auth.Login;

public class LoginUserRequestValidator: AbstractValidator<LoginUserCommand>
{
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.LoginRequest.UserName)
            .NotEmpty().WithMessage("Username is required")
            .NotNull().WithMessage("Username is required");

        RuleFor(x => x.LoginRequest.Password)
            .NotEmpty().WithMessage("Password is required")
            .NotNull().WithMessage("Password is required");
    }
}
