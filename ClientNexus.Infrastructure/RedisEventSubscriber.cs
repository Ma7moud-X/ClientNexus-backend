using ClientNexus.Domain.Interfaces;
using StackExchange.Redis;

namespace ClientNexus.Infrastructure;

public class RedisEventSubscriber : IEventSubscriber
{
    private readonly IConnectionMultiplexer _redis;
    private RedisChannel _redisChannel = default!;
    private bool _disposed = false;
    private Action<RedisChannel, RedisValue>? _subscriptionHandler = null;

    public RedisEventSubscriber(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task SubscribeAsync(string channel, Action<string> messageHandler)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RedisEventSubscriber));
        }

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

    public void Unsubscribe()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RedisEventSubscriber));
        }

        if (_subscriptionHandler is null)
        {
            throw new Exception("Not subscribed to any channel.");
        }

        var subscriber = _redis.GetSubscriber();
        subscriber.Unsubscribe(_redisChannel, _subscriptionHandler);
        _subscriptionHandler = null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RedisEventSubscriber));
        }

        if (_subscriptionHandler is null)
        {
            return;
        }

        var subscriber = _redis.GetSubscriber();
        subscriber.Unsubscribe(_redisChannel, _subscriptionHandler);
        _disposed = true;
    }
}
