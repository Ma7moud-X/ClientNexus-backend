using System.Threading.Channels;
using ClientNexus.Domain.Interfaces;
using StackExchange.Redis;

namespace ClientNexus.Infrastructure;

public class RedisEventListener : IEventListener
{
    private readonly IEventSubscriber _eventSubscriber;
    private readonly IConnectionMultiplexer _redis;
    private string? _channel = null;
    private readonly Channel<string> _messageQueue = Channel.CreateUnbounded<string>();

    public RedisEventListener(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _eventSubscriber = new RedisEventSubscriber(redis);
    }

    public async Task<string> ListenAsync(CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException(
                "You must subscribe to a channel before listening."
            );
        }

        return await _messageQueue.Reader.ReadAsync(cancellationToken);
    }

    public async Task SubscribeAsync(string channel)
    {
        if (string.IsNullOrEmpty(channel))
        {
            throw new ArgumentException("Channel name cannot be null or empty.", nameof(channel));
        }

        if (_channel is not null)
        {
            throw new InvalidOperationException("Already subscribed to a channel.");
        }

        _channel = channel;

        try
        {
            await _eventSubscriber.SubscribeAsync(
                _channel,
                async (message) =>
                {
                    await _messageQueue.Writer.WriteAsync(message);
                }
            );
        }
        catch (Exception ex)
        {
            // Handle exceptions related to subscription
            throw new InvalidOperationException(
                $"Failed to subscribe to channel '{_channel}'.",
                ex
            );
        }
    }

    public void Dispose()
    {
        _eventSubscriber.Dispose();

        // TODO: Dispose of the message queue if necessary
    }
}
