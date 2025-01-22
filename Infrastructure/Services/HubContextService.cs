using Application.Services;
using Infrastructure.Realtime;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
namespace Infrastructure.Services
{
    public class HubContextService : IHubContextService
    {
        private readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();
        private readonly IHubContext<VideoCallHub> _hubContext;

        public HubContextService(IHubContext<VideoCallHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public void AddConnection(string userId, string connectionId)
        {
            _userConnections[userId] = connectionId;
        }

        public async Task AddToGroupAsync(string connectionId, string groupName)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
        }

        public void RemoveConnection(string userId)
        {
            _userConnections.TryRemove(userId, out _);
        }

        public string GetConnectionId(string userId)
        {
            _userConnections.TryGetValue(userId, out var connectionId);
            return connectionId;
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