using System;
using Domain.Entities;
using Domain.Exceptions.NotFound;
using Domain.Repositories;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.Features.Messaging.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Response<SendMessageResponseDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IParticipantRepository _participantRepository;
    public SendMessageCommandHandler( IMessageRepository messageRepository, IChatRepository chatRepository, IParticipantRepository participantRepository)
    {
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
        _participantRepository = participantRepository;
    }
    public async Task<Response<SendMessageResponseDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetByIdAsync(request.request.ChatId);
        if (chat == null)
        {
                throw new NotFoundCustomException<Chat>(request.request.ChatId);
        }
        var participantId = await _participantRepository.GetParticipantIdByUserIdAndChatIdAsync(request.request.UserId, request.request.ChatId);
        var message = new Message(new Guid(),request.request.Content, request.request.Type, participantId, request.request.ChatId);
        await _messageRepository.CreateAsync(message);
        var response = new SendMessageResponseDto(message.Id, message.Content, message.Type, message.SentAt, message.SenderId, message.ChatId);
        return new Response<SendMessageResponseDto>(response);
    }
}
