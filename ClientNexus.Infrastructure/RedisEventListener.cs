using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Infrastructure;

public class RedisEventListener : IEventListener
{
    private readonly IEventSubscriber _eventSubscriber;
    private string? _channel = null;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly Queue<string> _messageQueue = new Queue<string>();

    public RedisEventListener(IEventSubscriber eventSubscriber)
    {
        _eventSubscriber = eventSubscriber;
    }

    public async Task<string> ListenAsync(CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException(
                "You must subscribe to a channel before listening."
            );
        }

        await _semaphore.WaitAsync(cancellationToken);
        return _messageQueue.Dequeue();
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
                (message) =>
                {
                    _messageQueue.Enqueue(message);
                    _semaphore.Release();
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
        _semaphore.Wait();
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}
