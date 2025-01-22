using System;
using Application.Features.Messaging.GetMessagesFromChat;
using Application.Features.Messaging.SendMessage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Helper;
using WebApi.Hubs;

namespace WebApi.Controllers.Message;

[ApiController]
[Route("api/[controller]")]
public class MessageController: ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<ChatHub> _chatHub;
    public MessageController(IMediator mediator, IHubContext<ChatHub> chatHub)
    {
        _mediator = mediator;
        _chatHub = chatHub;
    }
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("GetMessagesFromChat/{chatId}")]

    public async Task<IActionResult> GetMessagesFromChat([FromRoute] Guid chatId)
    {
        var query = new GetMessagesFromChatQuery(chatId);
        var response = await _mediator.Send(query);

        if (response.Succeeded)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPost("SendMessage")]
public async Task<IActionResult> CreateMessage([FromBody]SendMessageRequestDto  request)
    {
        var userId = TokenUtils.GetUserIdFromContext(HttpContext);
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }
         Console.WriteLine("User ID: " + userId);
            var command = new SendMessageCommand(new SendMessageRequest(request.Content, request.Type, userId.Value, request.ChatId));
            var response = await _mediator.Send(command);

            if (response.Succeeded)
            {
                await _chatHub.Clients.Group(request.ChatId.ToString()).SendAsync("ReceiveMessage", userId.ToString(), request.Content);

                return Ok(response);
            }

        return BadRequest();
    }
}
