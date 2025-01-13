using System;
using backend.src.Models;
using FluentValidation;

namespace backend.src.Validators.User;

public class UserRegistorRequestValidator: AbstractValidator<UserRegister>
{
 public UserRegistorRequestValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
    }
}
