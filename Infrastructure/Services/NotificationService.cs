
using System.Text;
using Application.Services;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        public NotificationService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory
                .CreateConnectionAsync()
                .GetAwaiter()
                .GetResult();
            _channel = _connection
                .CreateChannelAsync()
                .GetAwaiter()
                .GetResult();
            _channel.QueueDeclareAsync(queue: "notifications", 
                durable: false, exclusive: false, 
                autoDelete: false, 
                arguments: null);
        }
        public async Task AddPendingNotificationAsync(string userId, string notification)
        {
            var message = new { UserId = userId, Notification = notification };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            await _channel.BasicPublishAsync(exchange: "", 
            routingKey: "notifications",
            body: body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}