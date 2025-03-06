using ClientNexus.Domain.Interfaces;
using ClientNexus.Infrastructure;
using StackExchange.Redis;

namespace ClientNexus.API.Extensions;

public static class CacheExtensions
{
    public static void AddRedisCache(this IServiceCollection services)
    {
        string? redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STR");
        if (redisConnectionString is null)
        {
            throw new InvalidOperationException(
                "REDIS_CONNECTION_STR environment variable is not set."
            );
        }

        try
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Failed to connect to Redis.");
        }

        services.AddScoped<ICache, RedisCache>();
    }
}
