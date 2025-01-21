using Application.Services;
using Infrastructure.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services
{
    public class HubContextService : IHubContextService
    {
        private readonly IHubContext<VideoCallHub> _hubContext;

        public HubContextService(IHubContext<VideoCallHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task AddToGroupAsync(string connectionId, string groupName)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
        }

        public string GetConnectionId(string userId)
        {
            return Guid.NewGuid().ToString();
        }

        public async Task RemoveFromGroupAsync(string connectionId, string groupName)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }

        public async Task SendToGroupAsync(string groupName, string method, params object[] args)
        {
            await _hubContext.Clients.Group(groupName).SendAsync(method, args);
        }
    }
}