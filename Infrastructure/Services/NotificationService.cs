
using System.Text;
using Application.Services;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _cache;
        public NotificationService(
            IConfiguration configuration,
            IDistributedCache cache)
        {
            _configuration = configuration;
            _cache = cache;

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

            // Serialize the notification to JSON format
            var serializedNotification = JsonConvert.SerializeObject(new { Notification = notification });

            // Append the notification to the user's list of pending notifications in Redis
            
            var redisKey = $"notifications:{userId}";

            // Retrieve existing notifications, if any
            var existingNotificationsJson = await _cache.GetStringAsync(redisKey);
            var notifications = existingNotificationsJson != null
                ? JsonConvert.DeserializeObject<List<string>>(existingNotificationsJson)
                : new List<string>();

            // Add the new notification to the list
            notifications.Add(notification);

            // Serialize the updated list back to JSON
            var updatedNotificationsJson = JsonConvert.SerializeObject(notifications);

            // Store the updated list in Redis with an expiration (optional)
            await _cache.SetStringAsync(redisKey, updatedNotificationsJson, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });

            await _channel.BasicPublishAsync(exchange: "", 
            routingKey: "notifications",
            body: body);
        }

        public async Task ClearNotificationsAsync(string userId)
        {
            // Redis key for the user's notifications
            var redisKey = $"notifications:{userId}";

            // Remove the key from Redis to clear notifications
            await _cache.RemoveAsync(redisKey);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        public async Task<List<string>> GetNotificationsAsync(string userId)
        {
            var redisKey = $"notifications:{userId}";

            // Retrieve existing notifications
            var notificationsJson = await _cache.GetStringAsync(redisKey);

            // If no notifications exist, return an empty list
            if (string.IsNullOrEmpty(notificationsJson))
            {
                return new List<string>();
            }

            // Deserialize the notifications JSON into a list of strings
            var notifications = JsonConvert.DeserializeObject<List<string>>(notificationsJson);

            return notifications;
        }

        public async Task SaveNotificationAsync(string userId, string notification)
        {
            // Redis key for the user's notifications
            var redisKey = $"notifications:{userId}";

            // Retrieve existing notifications, if any
            var existingNotificationsJson = await _cache.GetStringAsync(redisKey);
            var notifications = existingNotificationsJson != null
                ? JsonConvert.DeserializeObject<List<string>>(existingNotificationsJson)
                : new List<string>();

            // Add the new notification to the list
            notifications.Add(notification);

            // Serialize the updated list back to JSON
            var updatedNotificationsJson = JsonConvert.SerializeObject(notifications);

            // Store the updated list in Redis with an expiration (optional)
            await _cache.SetStringAsync(redisKey, updatedNotificationsJson, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) // Set expiration time as needed
            });
        }
    }
}