using System;
using System.Text.Json;
using ClientNexus.Application.DTO;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class OfferListener : IDisposable
{
    private readonly IEventListener _eventListener;
    private readonly int _serviceId;
    private bool _isSubscribed = false;
    private bool _manuallyDisposed = false;
    private const string _keyTemplate = "clientnexus:services:{0}:";

    public OfferListener(IEventListener eventListener, int serviceId)
    {
        ArgumentNullException.ThrowIfNull(eventListener);
        _eventListener = eventListener;
        _serviceId = serviceId;
    }

    public async Task<ClientOfferDTO> ListenForOfferAsync(CancellationToken cancellationToken)
    {
        if (!_isSubscribed)
        {
            _isSubscribed = true;
            await _eventListener.SubscribeAsync(
                $"{string.Format(_keyTemplate, _serviceId)}offersChannel"
            );
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
            $"{string.Format(_keyTemplate, _serviceId)}offersChannel"
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
            _eventListener.Dispose();
        }
    }
}
