using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Friendship.AcceptFriendshipInvitation;
using Application.Features.Friendship.GetFriendList;
using Application.Features.Friendship.RejectFriendshipInvitation;
using Application.Features.Friendship.SendFriendshipInvitation;
using Domain.Enums;
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

        [HttpGet("initiated")]
        public async Task<IActionResult> GetInitiatedFriendList(
            [FromQuery] Guid userId,
            [FromQuery] FriendshipStatus status)
        {
            var user = await _mediator.Send(new GetFriendListQuery(userId, status, FriendshipFilter.Initiated));
            return Ok(user);
        }

        [HttpGet("received")]
        public async Task<IActionResult> GetReceivedFriendList(
            [FromQuery] Guid userId,
            [FromQuery] FriendshipStatus status)
        {
            var user = await _mediator.Send(new GetFriendListQuery(userId, status, FriendshipFilter.Received));
            return Ok(user);
        }

        [HttpGet("friendlist")]
        public async Task<IActionResult> GetFriendList(
            [FromQuery] Guid userId,
            [FromQuery] FriendshipStatus status)
        {
            var user = await _mediator.Send(new GetFriendListQuery(userId, status, FriendshipFilter.Both));
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

        [HttpPatch("reject")]
        public async Task<IActionResult> RejectFriendshipInvitation(
            [FromQuery] Guid friendshipId)
        {
            await _mediator.Send(new RejectFriendshipInvitationCommand(friendshipId));
            return Ok();
        }

        [HttpPatch("approve")]
        public async Task<IActionResult> ApproveFriendshipInvitation(
            [FromQuery] Guid friendshipId)
        {
            await _mediator.Send(new AcceptFriendshipInvitationCommand(friendshipId));
            return Ok();
        }
    }
}