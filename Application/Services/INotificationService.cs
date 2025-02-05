using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface INotificationService
    {
        public Task AddPendingNotificationAsync(string userId, string notification);
        Task SaveNotificationAsync(string userId, string notification);
        Task<List<string>> GetNotificationsAsync(string userId);
        Task ClearNotificationsAsync(string userId);

        public void Dispose();
    }
}