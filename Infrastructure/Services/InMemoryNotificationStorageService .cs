using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services;

namespace Infrastructure.Services
{
    public class InMemoryNotificationStorageService :INotificationStorageService
    {
        private readonly ConcurrentDictionary<string, List<string>> _storage = new();

        public Task SaveNotificationAsync(string userId, string notification)
        {
            if (!_storage.ContainsKey(userId))
            {
                _storage[userId] = new List<string>();
            }

            _storage[userId].Add(notification);
            return Task.CompletedTask;
        }

        public Task<List<string>> GetNotificationsAsync(string userId)
        {
            _storage.TryGetValue(userId, out var notifications);
            return Task.FromResult(notifications ?? new List<string>());
        }

        public Task ClearNotificationsAsync(string userId)
        {
            _storage.TryRemove(userId, out _);
            return Task.CompletedTask;
        }
    }
}