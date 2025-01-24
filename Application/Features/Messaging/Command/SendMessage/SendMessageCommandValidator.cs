using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Domain.Repositories;
using Application.Features.Messaging.SendMessage;

namespace Application.Features.Messaging.Command.SendMessage
{
    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        private readonly IChatRepository _chatRepository;

        public SendMessageCommandValidator(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;

            RuleFor(x => x.request.Content).NotEmpty();
            RuleFor(x => x.request.Type).IsInEnum();
            RuleFor(x => x.request.UserId).NotEmpty();
            RuleFor(x => x.request.ChatId).NotEmpty();
            RuleFor(x => x)
                .MustAsync(UserBelongsToChat)
                .WithMessage(x => $"User: {x.request.UserId} does not belong to the chat: {x.request.ChatId}");
        }

        private async Task<bool> UserBelongsToChat(SendMessageCommand command, CancellationToken cancellationToken)
        {
            return await _chatRepository.IsUserInChatAsync(command.request.ChatId, command.request.UserId);
        }
    }
}