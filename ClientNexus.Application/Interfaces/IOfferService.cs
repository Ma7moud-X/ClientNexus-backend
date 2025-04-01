using ClientNexus.Application.Domain;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Models;

namespace ClientNexus.Application.Interfaces;

public interface IOfferService
{
    Task CreateOfferAsync(
        int serviceId,
        decimal Price,
        ServiceProviderOverview serviceProvider,
        TravelDistance travelDistance,
        TimeSpan offerTTL
    );

    Task<decimal?> GetOfferPriceAsync(int serviceId, int ServiceProviderId);
    Task<bool> AllowOffersAsync<T>(T service, int timeoutInMin = 16)
        where T : ServiceProviderServiceDTO;
}
