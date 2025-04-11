using ClientNexus.Application.Constants;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class MissedOfferGetterService : IMissedOfferGetterService
{
    private readonly ICache _cache;

    public MissedOfferGetterService(ICache cache)
    {
        ArgumentNullException.ThrowIfNull(cache);
        _cache = cache;
    }

    public async Task<IEnumerable<ClientOfferDTO?>?> GetAllAsync(int serviceId)
    {
        return await _cache.GetListObjectAsync<ClientOfferDTO>(
            string.Format(CacheConstants.MissedOffersKeyTemplate, serviceId)
        );
    }
}
