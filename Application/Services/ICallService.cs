using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IVideoCallService
    {
        Task StartCallAsync(string roomId, string userId);
        Task JoinCallAsync(string connectionId, string roomId, string userId);
        Task SendSignalAsync(string connectionId, string roomId, string userId, string signal);
        Task LeaveCallAsync(string connectionId, string roomId, string userId);
    }
}