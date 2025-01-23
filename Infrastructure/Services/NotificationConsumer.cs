using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services;
using Infrastructure.Realtime;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Services
{
    public class NotificationConsumer : INotificationConsumer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IHubContextService _hubContextService;
        private readonly IHubContext<VideoCallHub> _hubContext;
        private readonly INotificationStorageService _notificationStorage;

        public NotificationConsumer(
            IHubContextService hubContextService,
            IHubContext<VideoCallHub> hubContext,
            INotificationStorageService notificationStorage)
        {
            _hubContextService = hubContextService;
            _hubContext = hubContext;
            _notificationStorage = notificationStorage;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory
                .CreateConnectionAsync()
                .GetAwaiter()
                .GetResult();
            _channel = _connection
                .CreateChannelAsync()
                .GetAwaiter()
                .GetResult(); //;
            _channel.QueueDeclareAsync(queue: "notifications", 
                durable: false, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null);
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task StartConsuming()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var notification = JsonConvert.DeserializeObject<NotificationMessage>(message);

                var connectionId = _hubContextService.GetConnectionId(notification.UserId);
                if (connectionId != null)
                {
                    // Deliver the notification immediately
                    await _hubContext.Clients.User(notification.UserId).SendAsync("ReceiveNotification", notification.Notification);
                }
                else
                {
                    // Store the notification for later delivery
                    await _notificationStorage.SaveNotificationAsync(notification.UserId, notification.Notification);
                }
                // Deliver the notification to the user
            };

            await _channel.BasicConsumeAsync(queue: "notifications", autoAck: true, consumer: consumer);
        }

        private class NotificationMessage
        {
            public string UserId { get; set; }
            public string Notification { get; set; }
        }
    }
}