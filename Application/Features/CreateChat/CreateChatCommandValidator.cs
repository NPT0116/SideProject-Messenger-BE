using System;
using FluentValidation;
using Domain.Repositories;

namespace Application.Features.Messaging.CreateChat
{
    public class CreateChatCommandValidator : AbstractValidator<CreateChatCommand>
    {
        private readonly IUserRepository _userRepository;

        public CreateChatCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.CreateChatRequestDto.User1)
                .NotEmpty().WithMessage("User1 ID is required")
                .NotEqual(Guid.Empty).WithMessage("User1 ID is required")
                .MustAsync(UserExists).WithMessage("User1 does not exist");

            RuleFor(x => x.CreateChatRequestDto.User2)
                .NotEmpty().WithMessage("User2 ID is required")
                .NotEqual(Guid.Empty).WithMessage("User2 ID is required")
                .MustAsync(UserExists).WithMessage("User2 does not exist");
        }

        private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
        {
            return await _userRepository.ExistsAsync(userId);
        }
    }
}