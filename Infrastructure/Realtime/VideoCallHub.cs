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
            await Clients.Others.SendAsync("UserJoined", userId);
        }

        public async Task SendSignal(string roomId, string userId, string signal)
        {
            Console.WriteLine("Sending signal is called");
            try
            {
                Console.WriteLine($"SendSignal invoked with roomId: {roomId}, userId: {userId}, signal: {signal}");

                // Log connection details
                Console.WriteLine($"Context.ConnectionId: {Context.ConnectionId}");

                // Check if the parameters are null or empty
                if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(signal))
                {
                    Console.WriteLine("Invalid parameters passed to SendSignal.");
                    throw new ArgumentException("RoomId, userId, or signal is null or empty.");
                }

                // Send the signal using mediator
                await _mediator.Send(new SendSignalCommand(Context.ConnectionId, roomId, userId, signal));
                Console.WriteLine("Signal sent via mediator.");

                // Notify the group
                await _hubContextService.SendToGroupAsync(roomId, "ReceiveSignal", Context.ConnectionId, signal);
                Console.WriteLine("Signal sent to group.");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SendSignal: {ex.Message}");
                throw; // Re-throw the exception to inform the client
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
            var pendingNotifications = await _notificationService.GetNotificationsAsync(userId);

            // Send all pending notifications to the client
            var connectionId = _hubContextService.GetConnectionId(userId);
            foreach (var notification in pendingNotifications)
            {
                // Check if the notification is in the "CallStarted" format
                if (notification.StartsWith("CallStarted:"))
                {
                    // Parse callerId and roomId from the notification
                    var parts = notification.Split(':');
                    if (parts.Length == 3)
                    {
                        var callerId = parts[1];
                        var roomId = parts[2];

                        // Notify the frontend about the call
                        await Clients.Client(connectionId).SendAsync("CallStarted", callerId, roomId);
                    }
                }
                else
                {
                    // Send other types of notifications
                    await Clients.Client(connectionId).SendAsync("ReceiveNotification", notification);
                }
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

        // public async Task LeaveCall(string roomId, string userId)
        // {
        //     await _mediator.Send(new LeaveCallCommand(Context.ConnectionId, roomId, userId));
        //     await _hubContextService.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        //     await _hubContextService.SendToGroupAsync(roomId, "UserLeft", userId);
        // }

        public async Task SendOffer(string receiverId, string offer)
        {
            await Clients.User(receiverId).SendAsync("ReceiveOffer", offer);
        }

        public async Task SendAnswer(string callerId, string answer)
        {
            await Clients.User(callerId).SendAsync("ReceiveAnswer", answer);
        }

        public async Task SendIceCandidate(string receiverId, string candidate)
        {
            await Clients.User(receiverId).SendAsync("ReceiveIceCandidate", candidate);
        }

        public async Task LeaveCall(string userId)
        {
            // Notify the other user that the call has ended
            await Clients.Others.SendAsync("UserLeft", userId);
        }

        public async Task DeclineCall(string callerId)
        {
            // Notify the caller that the call was declined
            await Clients.Client(callerId).SendAsync("CallDeclined");
        }
    }
}