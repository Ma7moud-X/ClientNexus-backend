using ClientNexus.Domain.Interfaces;
using StackExchange.Redis;

namespace ClientNexus.Infrastructure;

public class RedisEventPublisher : IEventPublisher
{
    private readonly IConnectionMultiplexer _redis;
    public RedisEventPublisher(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task PublishAsync(string channel, string message)
    {
        if (string.IsNullOrEmpty(channel))
        {
            throw new ArgumentException("Topic cannot be null or empty.", nameof(channel));
        }

        var subscriber = _redis.GetSubscriber();
        var redisChannel = RedisChannel.Literal(channel);
        await subscriber.PublishAsync(redisChannel, message);
    }
}
