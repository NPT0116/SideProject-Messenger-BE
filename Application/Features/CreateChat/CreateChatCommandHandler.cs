using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Application.Features.Messaging.CreateChat
{
    public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Response<Chat>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IParticipantRepository _participantRepository;

        public CreateChatCommandHandler(IChatRepository chatRepository, IParticipantRepository participantRepository)
        {
            _chatRepository = chatRepository;
            _participantRepository = participantRepository;
        }

        public async Task<Response<Chat>> Handle(CreateChatCommand request, CancellationToken cancellationToken)
        {
            // Validate input


            var user1Id = request.CreateChatRequestDto.User1;
            var user2Id = request.CreateChatRequestDto.User2;
            var existingChat = await _chatRepository.GetChatBetweenUsersAsync(user1Id, user2Id);
            if (existingChat != null)
            {
                return new Response<Chat>(existingChat)
                {
                    Succeeded = true,
                    Message = "Chat room already exists"
                };
            }
            // Create new Chat entity
            var chat = new Chat(Guid.NewGuid(), ChatType.Private);

            // Create Participant entities for the two users
            var participant1 = new Participant(Guid.NewGuid(), user1Id.ToString(), chat.Id);
            var participant2 = new Participant(Guid.NewGuid(), user2Id.ToString(), chat.Id);

            // Save Chat and Participants to the database
            await _chatRepository.CreateAsync(chat);
            await _participantRepository.CreateAsync(participant1);
            await _participantRepository.CreateAsync(participant2);

            // Return response with the created Chat
            return new Response<Chat>(chat)
            {
                Succeeded = true,
                Message = "Chat room created successfully"
            };
        }
    }
}