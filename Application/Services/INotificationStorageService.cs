using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface INotificationStorageService
    {
        Task SaveNotificationAsync(string userId, string notification);
        Task<List<string>> GetNotificationsAsync(string userId);
        Task ClearNotificationsAsync(string userId);
    }
}