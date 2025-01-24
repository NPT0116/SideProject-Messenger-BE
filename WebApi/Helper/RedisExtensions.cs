using System;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace WebApi.Helper;

public static class RedisExtensions
{
    public static async Task<IEnumerable<string>> GetKeysAsync(IConnectionMultiplexer connectionMultiplexer, string pattern)
    {
        var server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern).Select(k => k.ToString());

        return await Task.FromResult(keys);
    }
    
}