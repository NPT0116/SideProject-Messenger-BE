using System;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using MediatR;

namespace Application.Features.Messaging.SendMessage;

public record SendMessageResponseDto(Guid Id, string Content, MessageType Type, DateTime CreatedAt, Guid ParticipantId, Guid ChatId);
public record SendMessageRequestDto(string Content, MessageType Type, Guid ChatId);
public record SendMessageRequest(string Content, MessageType Type, Guid UserId, Guid ChatId);
public record SendMessageCommand(SendMessageRequest request) : IRequest<Response<SendMessageResponseDto>>;
