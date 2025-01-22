using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Realtime
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Extract the user ID from the connection context (e.g., from claims or query string)
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}