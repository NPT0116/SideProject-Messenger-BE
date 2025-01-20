using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Realtime
{
    public class VideoCallHub : Hub
    {
        public async Task StartCall(string roomId, string userId)
        {
            await Clients.Group(roomId).SendAsync("CallStarted", userId);
        }
        public async Task JoinCall(string roomId, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("UserJoined", userId);
        }

        public async Task SendSignal(string roomId, string userId, string signal)
        {
            await Clients.Group(roomId).SendAsync("ReceiveSignal", Context.ConnectionId, signal);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Handle user leaving the room
            // You might need to track which room the user was in
            await base.OnDisconnectedAsync(exception);
        }

        public async Task LeaveCall(string roomId, string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("UserLeft", userId);
        }
    }
}