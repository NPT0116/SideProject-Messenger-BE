
using System.Text;
using Application.Services;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IConfiguration _configuration;
        public NotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
            var hostName = _configuration["RabbitMQ:HostName"];
            var userName = _configuration["RabbitMQ:UserName"];
            var password = _configuration["RabbitMQ:Password"];
            Console.WriteLine($"RabbitMQ HostName: {hostName}");
            Console.WriteLine($"RabbitMQ UserName: {userName}");
            Console.WriteLine($"RabbitMQ Password: {password}");
            var factory = new ConnectionFactory() 
            { 
                HostName = hostName ?? "localhost",
                UserName = userName ?? "guest",
                Password = password ?? "guest", 
            };
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