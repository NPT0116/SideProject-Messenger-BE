using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface INotificationService
    {
        public Task AddPendingNotificationAsync(string userId, string notification);

        public void Dispose();
    }
}