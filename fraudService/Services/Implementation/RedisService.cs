using fraudService.Services.Abstractions;
using Infrastructure.Persistence;
using StackExchange.Redis;

namespace fraudService.Services.Implementation;

public class RedisService : ICacheService
{
    private readonly IDatabase _redis;
    private readonly AppDbContext _dbContext;

    public RedisService(
        IConnectionMultiplexer connectionMultiplexer,
        AppDbContext dbContext)
    {
        _redis = connectionMultiplexer.GetDatabase();
        _dbContext = dbContext;
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        await _redis.StringSetAsync(key, value, expiry);
    }

    public async Task<string?> GetAsync(string key)
    {
        return await _redis.StringGetAsync(key);
    }
}