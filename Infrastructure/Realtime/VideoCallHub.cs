using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Realtime
{
    public class VideoCallHub : Hub
    {
        public async Task JoinCall(string userId)
        {
            await Clients.Others.SendAsync("UserJoined", userId);
        }

        public async Task SendSignal(string userId, string signal)
        {
            await Clients.User(userId).SendAsync("ReceiveSignal", Context.ConnectionId, signal);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.SendAsync("UserLeft", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}