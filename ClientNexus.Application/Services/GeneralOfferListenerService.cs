using ClientNexus.Application.Constants;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class GeneralOfferListenerService : IGeneralOfferListenerService
{
    private readonly IChannelOfferListenerService _channelOfferListener;
    private readonly IMissedOfferGetterService _missedOfferGetter;
    private readonly ICache _cache;

    private IEnumerable<ClientOfferDTO?>? _missedOffers = null;
    private bool gotMissedOffers = false;
    private int _missedOfferIdx = 0;

    private int? _serviceId;
    private bool isClosed = false;

    public GeneralOfferListenerService(
        IChannelOfferListenerService channelOfferListener,
        IMissedOfferGetterService missedOfferGetter,
        ICache cache
    )
    {
        ArgumentNullException.ThrowIfNull(channelOfferListener);
        ArgumentNullException.ThrowIfNull(missedOfferGetter);

        _channelOfferListener = channelOfferListener;
        _missedOfferGetter = missedOfferGetter;
        _cache = cache;
    }

    public async Task CloseAsync(bool save = false, IOfferSaverService? offerSaverService = null)
    {
        if (_serviceId is null)
        {
            throw new InvalidOperationException("Please subscribe first.");
        }

        if (isClosed)
        {
            throw new InvalidOperationException("Listener is already closed.");
        }

        if (!save)
        {
            _missedOffers = null;
            await _cache.RemoveKeyAsync(
                $"{string.Format(CacheConstants.MissedOffersKeyTemplate, _serviceId)}"
            );
        }
        else
        {
            ArgumentNullException.ThrowIfNull(offerSaverService);

            if (_missedOffers is not null)
            {
                for (int idx = _missedOfferIdx; idx < _missedOffers.Count(); idx++)
                {
                    var offer = _missedOffers.ElementAt(idx);
                    if (offer is null)
                    {
                        continue;
                    }

                    if (offer.ExpiresAt >= DateTime.UtcNow)
                    {
                        continue;
                    }

                    await offerSaverService.SaveAsync(
                        offer,
                        (int)_serviceId,
                        DateTime.UtcNow - offer.ExpiresAt + TimeSpan.FromSeconds(30)
                    );
                }
            }
        }

        await _channelOfferListener.CloseAsync(save);
        isClosed = true;
    }

    public async Task<ClientOfferDTO> ListenAsync(CancellationToken cancellationToken)
    {
        if (_serviceId is null)
        {
            throw new InvalidOperationException("Please subscribe first.");
        }

        if (isClosed)
        {
            throw new InvalidOperationException("Listener is closed. Cannot listen for offers.");
        }

        if (!gotMissedOffers)
        {
            try
            {
                _missedOffers = await _missedOfferGetter.GetAllAsync((int)_serviceId);
                gotMissedOffers = true;
            }
            catch (Exception) { }

            if (gotMissedOffers)
            {
                await _cache.RemoveKeyAsync(
                    $"{string.Format(CacheConstants.MissedOffersKeyTemplate, _serviceId)}"
                );
            }
        }

        if (_missedOffers is not null && _missedOfferIdx < _missedOffers.Count())
        {
            ClientOfferDTO? offer = null;
            while (offer is null && _missedOfferIdx < _missedOffers.Count())
            {
                offer = _missedOffers.ElementAt(_missedOfferIdx++);
                if (DateTime.UtcNow >= offer?.ExpiresAt)
                { // TODO: check if timing is correct
                    offer = null;
                }
            }

            if (offer is not null)
            {
                return offer;
            }
        }

        if (_missedOfferIdx == _missedOffers?.Count())
        {
            _missedOffers = null;
        }

        return await _channelOfferListener.ListenAsync(cancellationToken);
    }

    public async Task SubscribeAsync(int serviceId)
    {
        if (_serviceId is not null)
        {
            throw new InvalidOperationException("Already subscribed to a service.");
        }

        await _channelOfferListener.SubscribeAsync(serviceId);
        _serviceId = serviceId;
    }
}
