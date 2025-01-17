using System;
using FluentValidation;

namespace Application.Users.Queries.GetMeUser;

public class GetMeUserCommandValidator: AbstractValidator<GetMeUserCommand>
{
    public GetMeUserCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required")
            .NotNull().WithMessage("Token is required");
    }
}
