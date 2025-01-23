using System;
using System.Threading.Tasks;
using Application.Features.Messaging.CreateChat;
using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Helper;
using WebApi.Hubs;

namespace WebApi.Controllers.Chat
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<ChatHub> _hubContext;

        private readonly IChatRepository _IChatRepository;

        public ChatController(IMediator mediator, IHubContext<ChatHub> hubContext, IChatRepository chatRepository)
        {
            _mediator = mediator;
            _hubContext = hubContext;
_IChatRepository = chatRepository;
        }

        [HttpPost("CreateChat")]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatRequestDto request)
        {
            var command = new CreateChatCommand(request);
            var response = await _mediator.Send(command);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("JoinChat/{chatId}")]
        public async Task<IActionResult> JoinChat([FromRoute] Guid chatId)
        {
            var userId = TokenUtils.GetUserIdFromContext(HttpContext).ToString();
            if (userId == null)
            {
                return Unauthorized("User ID not found in token.");
            }
            // Call the JoinChat method in ChatHub
            await _hubContext.Clients.User(userId).SendAsync("JoinChat", chatId.ToString());
            return Ok("Joined chat successfully.");
        }


        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("LeaveChat/{chatId}")]
        public async Task<IActionResult> LeaveChat([FromRoute] Guid chatId)
        {
         var userId = TokenUtils.GetUserIdFromContext(HttpContext).ToString();
            if (userId == null)
            {
                return Unauthorized("User ID not found in token.");
            }
            // Call the LeaveChat method in ChatHub
            await _hubContext.Clients.User(userId).SendAsync("LeaveChat", chatId.ToString());
            return Ok("Left chat successfully.");
        }

                [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetUsersInChat/{chatId}")]
        public async Task<IActionResult> GetUsersInChat([FromRoute] Guid chatId)
        {
            var users = await _IChatRepository.GetUsersInChatAsync(chatId);
            return Ok(users);
        }

    }

}