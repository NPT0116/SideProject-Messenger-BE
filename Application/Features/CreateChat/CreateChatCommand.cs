
using System;
using Domain.Entities;
using Domain.Utils;
using MediatR;

namespace Application.Features.Messaging.CreateChat;



public record CreateChatRequestDto (Guid User1, Guid User2);
public record CreateChatCommand (CreateChatRequestDto CreateChatRequestDto) : IRequest<Response<Chat>>;