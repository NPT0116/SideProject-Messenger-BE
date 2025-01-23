using System;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.Features.Messaging.GetMessagesFromChat;

public class GetMessagesFromChatQueryHandler : IRequestHandler<GetMessagesFromChatQuery, Response<List<MessageDto>>>
{
    private readonly IMessageRepository _messageRepository;
    public GetMessagesFromChatQueryHandler( IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }
    public async Task<Response<List<MessageDto>>> Handle(GetMessagesFromChatQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetMessagesByChatIdAsync(request.ChatId);
        var messageDtos = messages.Select(m => new MessageDto(m.Id, m.Content, m.Type, m.SenderId)).ToList();
        return new Response<List<MessageDto>>(messageDtos);
    }
}
