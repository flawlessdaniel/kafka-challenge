namespace fraudService.Services.Abstractions;

public interface ICacheService
{
    Task SetAsync(string key, string value, TimeSpan? expiry = null);
    Task<string?> GetAsync(string key);
}