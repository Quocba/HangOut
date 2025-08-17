namespace HangOut.API.Services.Interface;

public interface IRedisService
{
    Task<string?> GetStringAsync(string key);
    
    Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null);
}