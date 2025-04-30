using System.Threading.Channels;
using ClientNexus.Domain.Interfaces;
using Google.Apis.Util;

namespace ClientNexus.Infrastructure;

public class RedisEventListener : IEventListener
{
    private readonly IEventSubscriber _eventSubscriber;
    private readonly ICache _cache;
    private string? _channel = null;
    private readonly Channel<string> _messageQueue = Channel.CreateUnbounded<string>();
    private bool _closed = false;

    public RedisEventListener(IEventSubscriber eventSubscriber, ICache cache)
    {
        _cache = cache;
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

        try // TODO: unwrap from try-catch
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

    public async Task CloseAsync(bool save = false, string? saveToListAtKey = null)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException("Not subscribed to any channel.");
        }

        if (_closed)
        {
            throw new InvalidOperationException("Already closed.");
        }

        if (save && string.IsNullOrEmpty(saveToListAtKey))
        {
            throw new ArgumentException(
                "saveToListAtKey cannot be null or empty when save is true.",
                nameof(saveToListAtKey)
            );
        }

        _eventSubscriber.Unsubscribe();

        if (!save)
        {
            _closed = true;
            return;
        }

        while (true)
        {
            bool isRead = _messageQueue.Reader.TryRead(out var message);
            if (!isRead)
            {
                break;
            }

            if (message is null)
            {
                continue;
            }

            try // TODO: unwrap from try catch
            {
                await _cache.AddToListStringAsync(saveToListAtKey!, message);
            }
            catch (Exception ex)
            {
                ex.Data["unsaved"] = _messageQueue;
                // Handle exceptions related to saving messages
                throw new InvalidOperationException(
                    $"Failed to save message to list '{saveToListAtKey}'.",
                    ex
                );
            }
        }

        _closed = true;
    }
}
