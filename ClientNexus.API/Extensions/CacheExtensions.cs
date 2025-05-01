using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
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

        var cacheConnOptions = ConfigurationOptions.Parse(
            Environment.GetEnvironmentVariable("REDIS_CONNECTION_STR")!
        );
        cacheConnOptions.Password = Environment.GetEnvironmentVariable("REDIS_PASS")!;
        cacheConnOptions.User = Environment.GetEnvironmentVariable("REDIS_USER");
        cacheConnOptions.Ssl = true;
        cacheConnOptions.AllowAdmin = false;

        try
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(cacheConnOptions);
            });
        }
        catch (Exception ex)
        {
            throw new ConnectionEstablishmentException("Failed to connect to Redis.", ex);
        }

        services.AddTransient<ICache>(sp =>
        {
            var connection = sp.GetRequiredService<IConnectionMultiplexer>();
            return CacheTryCatchDecorator<ICache>.Create(new RedisCache(connection));
        });
        services.AddSingleton<IEventPublisher, RedisEventPublisher>();
        services.AddTransient<IEventSubscriber, RedisEventSubscriber>();
        services.AddTransient<IEventListener, RedisEventListener>();
    }
}
