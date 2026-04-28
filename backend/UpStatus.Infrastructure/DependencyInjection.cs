using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Infrastructure.Auth;
using UpStatus.Infrastructure.Caching;
using UpStatus.Infrastructure.Persistence;
using UpStatus.Infrastructure.Persistence.Mappings;
using UpStatus.Infrastructure.Persistence.Repositories;
using UpStatus.Infrastructure.Time;

namespace UpStatus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        MongoMappings.Register();

        services.Configure<MongoOptions>(configuration.GetSection("Mongo"));
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<RedisOptions>(configuration.GetSection("Redis"));

        services.AddSingleton<MongoContext>();
        services.AddSingleton<MongoIndexInitializer>();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redis = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
            return ConnectionMultiplexer.Connect(redis.ConnectionString);
        });
        services.AddSingleton<ICacheService, RedisCacheService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMonitorRepository, MonitorRepository>();
        services.AddScoped<ICheckRepository, CheckRepository>();
        services.AddScoped<IIncidentRepository, IncidentRepository>();

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
