using System;

using System;
using Domain;
using Domain.Repositories;
using Serilog;

namespace WebApi.Middlewares;

public class UpdateLastAccessMiddleware
{
    private readonly RequestDelegate _next;

    public UpdateLastAccessMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository, ITokenService tokenService)
    {
        // Lấy thông tin user từ request (ví dụ: từ token hoặc session)
        var userId = tokenService.GetUserIdFromToken(context.Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
        Log.Information("Middleware update last access activate", userId);

        if (userId.HasValue)
        {
            var user = await  userRepository.GetUserByIdAsync(userId.Value);
            if (user != null)
            {
                user.LastSeen = DateTime.UtcNow;
                await userRepository.UpdateUserAsync(user);
            }
            Log.Information("User {UserId} last seen updated", userId);
        }

        await _next(context); // Chuyển request đến middleware tiếp theo
    }
}