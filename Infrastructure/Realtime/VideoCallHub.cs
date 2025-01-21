using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.VideoCall.JoinCall;
using Application.Features.VideoCall.LeaveCall;
using Application.Features.VideoCall.SendSignal;
using Application.Features.VideoCall.StartCall;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Realtime
{
    public class VideoCallHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IHubContextService _hubContextService;

        public VideoCallHub(IMediator mediator, IHubContextService hubContextService)
        {
            _mediator = mediator;
            _hubContextService = hubContextService;
        }

        public async Task StartCall(string userId)
        {
            var roomId = await _mediator.Send(new StartCallCommand(userId));
            await _hubContextService.SendToGroupAsync(roomId, "CallStarted", userId);
        }

        public async Task JoinCall(string roomId, string userId)
        {
            await _mediator.Send(new JoinCallCommand(Context.ConnectionId, roomId, userId));
            await _hubContextService.AddToGroupAsync(Context.ConnectionId, roomId);
            await _hubContextService.SendToGroupAsync(roomId, "UserJoined", userId);
        }

        public async Task SendSignal(string roomId, string userId, string signal)
        {
            await _mediator.Send(new SendSignalCommand(Context.ConnectionId, roomId, userId, signal));
            await _hubContextService.SendToGroupAsync(roomId, "ReceiveSignal", Context.ConnectionId, signal);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Handle user leaving the room
            await base.OnDisconnectedAsync(exception);
        }

        public async Task LeaveCall(string roomId, string userId)
        {
            await _mediator.Send(new LeaveCallCommand(Context.ConnectionId, roomId, userId));
            await _hubContextService.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await _hubContextService.SendToGroupAsync(roomId, "UserLeft", userId);
        }
    }
}