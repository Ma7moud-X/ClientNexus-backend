using System;
using System.Runtime.Serialization;
using System.Text.Json;
using ClientNexus.Application.Constants;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class ChannelOfferListenerService : IChannelOfferListenerService
{
    private readonly IEventListener _eventListener;
    private int? _serviceId;
    private bool _isClosed = false;

    public ChannelOfferListenerService(IEventListener eventListener)
    {
        ArgumentNullException.ThrowIfNull(eventListener);
        _eventListener = eventListener;
    }

    public async Task CloseAsync(bool save = false)
    {
        if (_serviceId is null || _isClosed)
        {
            return;
        }

        await _eventListener.CloseAsync(
            save,
            string.Format(CacheConstants.MissedOffersKeyTemplate, _serviceId)
        );
        _isClosed = true;
    }

    public async Task<ClientOfferDTO> ListenAsync(CancellationToken cancellationToken)
    {
        if (_serviceId is null)
        {
            throw new InvalidOperationException("Service ID is not set. Please subscribe first.");
        }

        if (_isClosed)
        {
            throw new InvalidOperationException("Listener is closed. Cannot listen for offers.");
        }

        var offer = JsonSerializer.Deserialize<ClientOfferDTO>(
            await _eventListener.ListenAsync(cancellationToken)
        );

        if (offer is null)
        {
            throw new SerializationException(
                "Error deserializing offer. Offer was received in invalid format"
            );
        }

        return offer;
    }

    public async Task SubscribeAsync(int serviceId)
    {
        if (_serviceId is not null)
        {
            throw new InvalidOperationException("Already subscribed to a service.");
        }

        await _eventListener.SubscribeAsync(
            string.Format(CacheConstants.OffersChannelKeyTemplate, serviceId)
        );
        _serviceId = serviceId;
    }
}
