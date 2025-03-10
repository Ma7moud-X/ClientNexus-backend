using ClientNexus.Application.Domain;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Models;
using ClientNexus.Domain.Entities.Services;

namespace ClientNexus.Application.Interfaces;

public interface IOfferService
{
    Task<bool> CreateOfferAsync(
        int serviceId,
        double Price,
        ServiceProviderOverview serviceProvider,
        TravelDistance travelDistance
    );

    Task<ClientOfferDTO> GetOfferAsync(int serviceId, CancellationToken cancellationToken);
    Task<double> GetOfferPriceAsync(int serviceId, int ServiceProviderId);
    Task<bool> AllowOffersAsync<T>(int serviceId, T service)
        where T : Service;
}
