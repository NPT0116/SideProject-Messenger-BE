using System;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace WebApi.Middlewares
{
    public class UpdateLastAccessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDistributedCache _cache;

        public UpdateLastAccessMiddleware(RequestDelegate next, IServiceProvider serviceProvider, IDistributedCache cache)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Replace("Bearer ", "");
                var userId = tokenService.GetUserIdFromToken(token);

                if (userId.HasValue)
                {
                    Log.Information("Middleware update last access activated for user {UserId}", userId);

                    var cacheKey = $"LastSeen_{userId.Value}";
                    var lastSeen = DateTime.UtcNow;

                    // Store the last seen time in Redis
                    await _cache.SetStringAsync(cacheKey, lastSeen.ToString("o"));

                    Log.Information("User {UserId} last seen updated in cache", userId);
                }
            }

            await _next(context); // Pass the request to the next middleware
        }
    }
}
