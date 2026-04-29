using Microsoft.Extensions.Options;
using UpStatus.Api.Hubs;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Infrastructure.Caching;

namespace UpStatus.Api.Configuration;

public static class SignalRConfiguration
{
    public static IServiceCollection AddRealtime(this IServiceCollection services, IConfiguration configuration)
    {
        var redis = configuration.GetSection("Redis").Get<RedisOptions>();
        var connectionString = redis?.ConnectionString;

        var builder = services.AddSignalR();

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            builder.AddStackExchangeRedis(connectionString, options =>
            {
                options.Configuration.ChannelPrefix = StackExchange.Redis.RedisChannel.Literal(
                    string.IsNullOrWhiteSpace(redis!.InstanceName) ? "upstatus" : redis.InstanceName);
            });
        }

        services.AddSingleton<IHubNotifier, HubNotifier>();

        return services;
    }
}
