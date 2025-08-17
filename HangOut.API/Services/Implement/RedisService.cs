using HangOut.API.Services.Interface;
using StackExchange.Redis;

namespace HangOut.API.Services.Implement;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _redisConnection;

    public RedisService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
        _redisConnection = redis;
    }
    
    public async Task<string?> GetStringAsync(string key)
    {
        return await _db.StringGetAsync(key);
    }
    
    public async Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null)
    {
        return await _db.StringSetAsync(key, value, expiry);
    }
    
}