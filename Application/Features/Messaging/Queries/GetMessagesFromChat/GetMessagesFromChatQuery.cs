using System;
using Domain.Enums;
using Domain.Utils;
using MediatR;

namespace Application.Features.Messaging.GetMessagesFromChat;


public record MessageDto(Guid Id, string Content, MessageType Type, Guid SenderId);
public record GetMessagesFromChatQuery(Guid ChatId): IRequest<Response<List<MessageDto>>>;
