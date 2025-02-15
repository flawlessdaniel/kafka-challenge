using StackExchange.Redis;
using statusUpdatedService.Services.Abstractions;

namespace statusUpdatedService.Services.Implementations;

public class RedisService : ICacheService
{
    private readonly IDatabase _redis;

    public RedisService(
        IConnectionMultiplexer connectionMultiplexer)
    {
        _redis = connectionMultiplexer.GetDatabase();
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        await _redis.StringSetAsync(key, value, expiry);
    }

    public async Task<string?> GetAsync(string key)
    {
        return await _redis.StringGetAsync(key);
    }

    public async Task RemoveAsync(string key)
    {
        if (!(await _redis.KeyExistsAsync(key))) return;
        await _redis.KeyDeleteAsync(key);
    }
}