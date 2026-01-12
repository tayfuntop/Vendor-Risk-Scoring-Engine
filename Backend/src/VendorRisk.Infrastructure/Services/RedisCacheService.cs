using Newtonsoft.Json;
using StackExchange.Redis;
using VendorRisk.Application.Interfaces;

namespace VendorRisk.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly string _instanceName;

    public RedisCacheService(IConnectionMultiplexer redis, string instanceName)
    {
        _database = redis.GetDatabase();
        _instanceName = instanceName;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var fullKey = $"{_instanceName}{key}";
        var value = await _database.StringGetAsync(fullKey);

        if (value.IsNullOrEmpty)
            return null;

        return JsonConvert.DeserializeObject<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var fullKey = $"{_instanceName}{key}";
        var serialized = JsonConvert.SerializeObject(value);

        await _database.StringSetAsync(fullKey, serialized, expiration);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = $"{_instanceName}{key}";
        await _database.KeyDeleteAsync(fullKey);
    }
}
