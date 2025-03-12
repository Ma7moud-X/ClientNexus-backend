using System;
using ClientNexus.Application.DTO;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Listeners;

public class MissedOfferListener
{
    private readonly ICache _cache;
    public int ServiceId { get; init; }
    private const string _keyTemplate = "clientnexus:services:{0}:";

    public MissedOfferListener(ICache cache, int serviceId)
    {
        ArgumentNullException.ThrowIfNull(cache);
        _cache = cache;
        ServiceId = serviceId;
    }

    public async Task<ClientOfferDTO?> GetMissedOfferAsync()
    {
        return await _cache.LeftPopListObjectAsync<ClientOfferDTO>(
            $"{string.Format(_keyTemplate, ServiceId)}offersList"
        );
    }
}
