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
        private readonly INotificationService _notificationService;
        private readonly INotificationStorageService _notificationStorageService;

        public VideoCallHub(IMediator mediator, 
            IHubContextService hubContextService,
            INotificationService notificationService,
            INotificationStorageService notificationStorageService)
        {
            _mediator = mediator;
            _hubContextService = hubContextService;
            _notificationService = notificationService;
            _notificationStorageService = notificationStorageService;
        }

        public async Task StartCall(string friendId)
        {
            var callerId = Context.UserIdentifier; // Extract callerId from the context (JWT)
            var roomId = await _mediator.Send(new StartCallCommand(friendId));

            // Notify the caller about the generated roomId
            await Clients.Caller.SendAsync("RoomCreated", roomId);

            // Check if the friend is online
            var friendConnectionId = _hubContextService.GetConnectionId(friendId);
            if (friendConnectionId != null)
            {
                Console.WriteLine("Friend is online");
                await Clients.Client(friendConnectionId).SendAsync("CallStarted", callerId, roomId);
            }
            else
            {
                Console.WriteLine("Friend is offline");
                // Store the notification if the friend is offline
                await _notificationService.AddPendingNotificationAsync(friendId, $"CallStarted:{callerId}:{roomId}");
            }
        }

        public async Task JoinCall(string roomId, string userId)
        {
            await _mediator.Send(new JoinCallCommand(Context.ConnectionId, roomId, userId));
            await _hubContextService.AddToGroupAsync(Context.ConnectionId, roomId);
            await _hubContextService.SendToGroupAsync(roomId, "UserJoined", userId);
        }

        public async Task SendSignal(string roomId, string userId, string signal)
        {
            try
            {
                await _mediator.Send(new SendSignalCommand(Context.ConnectionId, roomId, userId, signal));
                await _hubContextService.SendToGroupAsync(roomId, "ReceiveSignal", Context.ConnectionId, signal);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SendSignal: {ex.Message}");
                throw;
            }
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("No userId provided");
                Context.Abort(); // Optionally reject the connection if userId is missing
                return;
            }

            Console.WriteLine($"Connected userId: {userId}");
            Console.WriteLine($"ConnectionId: {Context.ConnectionId}");

            _hubContextService.AddConnection(userId, Context.ConnectionId);
            var pendingNotifications = await _notificationStorageService.GetNotificationsAsync(userId);
            foreach (var notification in pendingNotifications)
            {
                await Clients.Caller.SendAsync("ReceiveNotification", notification);
            }

            // Clear delivered notifications
            await _notificationStorageService.ClearNotificationsAsync(userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            _hubContextService.RemoveConnection(userId);
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