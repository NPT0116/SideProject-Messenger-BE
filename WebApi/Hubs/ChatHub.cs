using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebApi.Hubs
{
    public class ChatHub : Hub
    {
    public async Task SendMessage(string chatId, string senderId, string content)
    {
        await Clients.Group(chatId).SendAsync("ReceiveMessage", senderId, content);
    }

    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task LeaveChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
    }
    }
}