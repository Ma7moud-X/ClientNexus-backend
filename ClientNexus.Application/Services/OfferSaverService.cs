using ClientNexus.Application.Constants;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services
{
    public class OfferSaverService : IOfferSaverService
    {
        private readonly ICache _cache;

        public OfferSaverService(ICache cache)
        {
            ArgumentNullException.ThrowIfNull(cache);
            _cache = cache;
        }

        public async Task<bool> SaveAsync(ClientOfferDTO offerDTO, int serviceId, TimeSpan timeout)
        {
            ArgumentNullException.ThrowIfNull(offerDTO);
            _cache.StartTransaction();
            var offersAddedCount = _cache.AddToListObjectAsync(
                string.Format(CacheConstants.MissedOffersKeyTemplate, serviceId),
                offerDTO
            );
            _ = _cache.SetExpiryAsync(
                string.Format(CacheConstants.MissedOffersKeyTemplate, serviceId),
                timeout
            );
            var result = await _cache.CommitTransactionAsync();

            if (!result || await offersAddedCount == 0)
            {
                return false;
            }
            
            return true;
        }
    }
}
