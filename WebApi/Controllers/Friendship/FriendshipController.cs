using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Friendship.GetFriendList;
using Application.Features.Friendship.SendFriendshipInvitation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Friendship
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendshipController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FriendshipController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetFriendList([FromQuery] Guid userId)
        {
            var user = await _mediator.Send(new GetFriendListQuery(userId));
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendshipInvitation(
            [FromQuery] Guid initiatorId, 
            [FromQuery] Guid receiverId)
        {
            var createFriendshipDto = new CreateFriendshipDto(initiatorId, receiverId);
            var result = await _mediator.Send(new SendFriendshipInvitationCommand(createFriendshipDto));
            return Created();
        }
    }
}