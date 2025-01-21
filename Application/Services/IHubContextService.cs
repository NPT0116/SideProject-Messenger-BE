using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IHubContextService
    {
        string GetConnectionId(string userId);
        Task AddToGroupAsync(string connectionId, string groupName);
        Task RemoveFromGroupAsync(string connectionId, string groupName);
        Task SendToGroupAsync(string groupName, string method, params object[] args);
    }
}