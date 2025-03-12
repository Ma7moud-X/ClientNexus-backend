using System;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Services;

namespace ClientNexus.Application.Listeners;

public class GeneralOfferListener
{
    private readonly ChannelOfferListener _channelOfferListener;
    private readonly MissedOfferListener _missedOfferListener;
    private bool checkMissedOffer = true;
    private bool subscribedForIncomingOffers = false;

    public GeneralOfferListener(
        ChannelOfferListener channelOfferListener,
        MissedOfferListener missedOfferListener,
        int serviceId
    )
    {
        ArgumentNullException.ThrowIfNull(channelOfferListener);
        ArgumentNullException.ThrowIfNull(missedOfferListener);

        if (
            channelOfferListener.ServiceId != serviceId
            || missedOfferListener.ServiceId != serviceId
        )
        {
            throw new ArgumentException("Service ID mismatch between listeners.");
        }

        _channelOfferListener = channelOfferListener;
        _missedOfferListener = missedOfferListener;
    }

    public async Task<ClientOfferDTO> ListenForOfferAsync(CancellationToken cancellationToken)
    {
        if (!subscribedForIncomingOffers)
        {
            await _channelOfferListener.SubscribeAsync();
            subscribedForIncomingOffers = true;
        }

        if (checkMissedOffer)
        {
            var missedOffer = await _missedOfferListener.GetMissedOfferAsync();
            if (missedOffer is not null)
            {
                return missedOffer;
            }
            else
            {
                checkMissedOffer = false;
            }
        }

        return await _channelOfferListener.ListenForOfferAsync(cancellationToken);
    }
}
