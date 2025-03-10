using ClientNexus.Domain.Interfaces;
using StackExchange.Redis;

namespace ClientNexus.Infrastructure;

public class RedisEventSubscriber : IEventSubscriber
{
    private readonly IConnectionMultiplexer _redis;
    private RedisChannel _redisChannel = default!;
    private Action<RedisChannel, RedisValue> _subscriptionHandler = default!;

    public RedisEventSubscriber(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task SubscribeAsync(string channel, Action<string> messageHandler)
    {
        if (string.IsNullOrEmpty(channel))
        {
            throw new ArgumentException("Channel cannot be null or empty.", nameof(channel));
        }

        if (_subscriptionHandler is not null)
        {
            throw new InvalidOperationException("Already subscribed to a channel.");
        }

        _redisChannel = RedisChannel.Literal(channel);
        _subscriptionHandler = (ch, message) =>
        {
            messageHandler(message.ToString());
        };

        var subscriber = _redis.GetSubscriber();
        await subscriber.SubscribeAsync(_redisChannel, _subscriptionHandler);
    }

    public void Dispose()
    {
        if (_subscriptionHandler is null)
        {
            return;
        }

        var subscriber = _redis.GetSubscriber();
        subscriber.Unsubscribe(_redisChannel, _subscriptionHandler);
    }
}
