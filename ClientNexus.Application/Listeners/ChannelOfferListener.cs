using System;
using System.Text.Json;
using ClientNexus.Application.DTO;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class ChannelOfferListener : IDisposable
{
    private readonly IEventListener _eventListener;
    public int ServiceId { get; init; }
    private bool _isSubscribed = false;
    private bool _manuallyDisposed = false;
    private const string _keyTemplate = "clientnexus:services:{0}:";

    public ChannelOfferListener(IEventListener eventListener, int serviceId)
    {
        ArgumentNullException.ThrowIfNull(eventListener);
        _eventListener = eventListener;
        ServiceId = serviceId;
    }

    public async Task SubscribeAsync()
    {
        if (_isSubscribed)
        {
            return;
        }

        _isSubscribed = true;
        await _eventListener.SubscribeAsync(
            $"{string.Format(_keyTemplate, ServiceId)}offersChannel"
        );
    }

    public async Task<ClientOfferDTO> ListenForOfferAsync(CancellationToken cancellationToken)
    {
        if (!_isSubscribed)
        {
            await SubscribeAsync();
        }

        var offer = JsonSerializer.Deserialize<ClientOfferDTO>(
            await _eventListener.ListenAsync(cancellationToken)
        );

        if (offer is null)
        {
            throw new Exception("Error deserializing offer. Offer was received in invalid format");
        }

        return offer;
    }

    public async Task CloseAsync(bool save = false)
    {
        if (!_isSubscribed)
        {
            return;
        }

        if (_manuallyDisposed)
        {
            return;
        }

        _manuallyDisposed = true;
        await _eventListener.CloseAsync(
            save,
            $"{string.Format(_keyTemplate, ServiceId)}offersChannel"
        );
    }

    public void Dispose()
    {
        if (_manuallyDisposed)
        {
            return;
        }

        if (_isSubscribed)
        {
            try
            {
                _eventListener.Dispose();
            }
            catch (Exception) { }
        }
    }
}
