using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using UpStatus.Application.Common.Abstractions;

namespace UpStatus.Infrastructure.Caching;

public sealed class RedisCacheService : ICacheService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IConnectionMultiplexer _multiplexer;
    private readonly RedisOptions _options;

    public RedisCacheService(IConnectionMultiplexer multiplexer, IOptions<RedisOptions> options)
    {
        _multiplexer = multiplexer;
        _options = options.Value;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var db = _multiplexer.GetDatabase();
        var value = await db.StringGetAsync(Prefix(key));

        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value!, SerializerOptions);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl, CancellationToken ct)
    {
        var db = _multiplexer.GetDatabase();
        var payload = JsonSerializer.Serialize(value, SerializerOptions);
        return db.StringSetAsync(Prefix(key), payload, ttl);
    }

    public Task RemoveAsync(string key, CancellationToken ct)
    {
        var db = _multiplexer.GetDatabase();
        return db.KeyDeleteAsync(Prefix(key));
    }

    private string Prefix(string key) =>
        string.IsNullOrWhiteSpace(_options.InstanceName)
            ? key
            : $"{_options.InstanceName}:{key}";
}
