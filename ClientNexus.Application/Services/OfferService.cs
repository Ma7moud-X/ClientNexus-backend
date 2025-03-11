using ClientNexus.Application.Domain;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Models;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class OfferService : IOfferService
{
    private readonly ICache _cache;
    private const string _keyTemplate = "clientnexus:services:{0}:";

    public OfferService(ICache cache)
    {
        _cache = cache;
    }

    public async Task<bool> AllowOffersAsync<T>(T service, int timeoutInMin = 16)
        where T : ServiceProviderServiceDTO
    {
        ArgumentNullException.ThrowIfNull(service);

        _cache.StartTransaction();

        var created = _cache.SetObjectAsync(
            $"{string.Format(_keyTemplate, service.ServiceId)}request",
            service,
            TimeSpan.FromMinutes(timeoutInMin),
            @override: false
        );

        // var listCreated = _cache.AddToListStringAsync(
        //     $"{string.Format(_keyTemplate, service.ServiceId)}offersList",
        //     "empty"
        // );
        // var listExpirySet = _cache.SetExpiryAsync(
        //     $"{string.Format(_keyTemplate, service.ServiceId)}offersList",
        //     TimeSpan.FromMinutes(timeoutInMin)
        // );
        // var listEmptied = _cache.LeftPopListStringAsync(
        //     $"{string.Format(_keyTemplate, service.ServiceId)}offersList"
        // );

        var hashCreated = _cache.SetHashStringAsync(
            $"{string.Format(_keyTemplate, service.ServiceId)}offersHash",
            "empty",
            "empty"
        );
        var hashEmptied = _cache.RemoveHashFieldAsync("empty", "empty");
        var hashExpirySet = _cache.SetExpiryAsync(
            $"{string.Format(_keyTemplate, service.ServiceId)}offersHash",
            TimeSpan.FromMinutes(timeoutInMin)
        );

        bool transactionCommitted;
        try
        {
            transactionCommitted = await _cache.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error committing transaction", ex);
        }

        return transactionCommitted && await created;
    }

    public Task<bool> CreateOfferAsync(
        int serviceId,
        double Price,
        ServiceProviderOverview serviceProvider,
        TravelDistance travelDistance
    )
    {
        throw new NotImplementedException();
    }

    public Task<ClientOfferDTO> GetOfferAsync(int serviceId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<double> GetOfferPriceAsync(int serviceId, int ServiceProviderId)
    {
        throw new NotImplementedException();
    }
}
