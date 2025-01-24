using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WebApi.Helper;

public class LastSeenSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<LastSeenSyncService> _logger;

    public LastSeenSyncService(IServiceProvider serviceProvider, IDistributedCache cache, IConnectionMultiplexer connectionMultiplexer, ILogger<LastSeenSyncService> logger)
    {
        _serviceProvider = serviceProvider;
        _cache = cache;
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SyncLastSeenData(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Sync every 5 minutes
        }
    }

    private async Task SyncLastSeenData(CancellationToken stoppingToken)
{
    using (var scope = _serviceProvider.CreateScope())
    {
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var db = _connectionMultiplexer.GetDatabase();

        // Lấy danh sách UserID từ Redis List
        var userIds = await db.ListRangeAsync("UserLastSeenList");
        foreach (var id in userIds)
        {
            var userId = Guid.Parse(id);
            var lastSeenString = await _cache.GetStringAsync($"LastSeen_{userId}");
            if (DateTime.TryParse(lastSeenString, out var lastSeen))
            {
                var user = await userRepository.GetUserByIdAsync(userId);
                if (user != null)
                {
                    user.LastSeen = lastSeen;
                    await userRepository.UpdateUserAsync(user);
                    await _cache.RemoveAsync($"LastSeen_{userId}");
                }
            }

            // Xóa UserID khỏi danh sách sau khi đồng bộ
            await db.ListRemoveAsync("UserLastSeenList", id);
        }

        _logger.LogInformation("Last seen data synced with the database");
    }
}

}