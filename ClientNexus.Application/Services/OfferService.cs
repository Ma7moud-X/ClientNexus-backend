using System.Text.Json;
using ClientNexus.Application.Domain;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Models;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class OfferService : IOfferService
{
    private readonly ICache _cache;
    private readonly IEventPublisher _eventPublisher;
    private const string _keyTemplate = "clientnexus:services:{0}:";

    public OfferService(ICache cache, IEventPublisher eventPublisher, IEventListener eventListener)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(eventPublisher);
        ArgumentNullException.ThrowIfNull(eventListener);

        _cache = cache;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> AllowOffersAsync<T>(T service, int timeoutInMin = 16)
        where T : ServiceProviderServiceDTO
    {
        ArgumentNullException.ThrowIfNull(service);
        if (timeoutInMin <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(timeoutInMin),
                "Timeout must be greater than 0"
            );
        }

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

    public async Task<bool> CreateOfferAsync(
        int serviceId,
        decimal price,
        ServiceProviderOverview serviceProvider,
        TravelDistance travelDistance
    )
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(travelDistance);

        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be greater than 0");
        }

        string offerStr = JsonSerializer.Serialize(
            new ClientOfferDTO
            {
                ServiceProviderId = serviceProvider.ServiceProviderId,
                FirstName = serviceProvider.FirstName,
                LastName = serviceProvider.LastName,
                Price = price,
                TimeForArrival = travelDistance.Duration,
                TimeUnit = travelDistance.DurationUnit,
                Rating = serviceProvider.Rating,
                YearsOfExperience = serviceProvider.YearsOfExperience,
                ImageUrl = serviceProvider.ImageUrl,
            }
        );

        try
        {
            TimeSpan? offerTTL = await _cache.GetTTLAsync(
                $"{string.Format(_keyTemplate, serviceId)}request"
            );

            if (offerTTL is null || offerTTL.Value.TotalMinutes <= 1.2)
            {
                return false;
            }

            long receivedByCount = await _eventPublisher.PublishAsync(
                $"{string.Format(_keyTemplate, serviceId)}offersChannel",
                offerStr
            );

            if (receivedByCount != 0)
            {
                await _cache.SetHashObjectAsync(
                    $"{string.Format(_keyTemplate, serviceId)}offersHash",
                    serviceProvider.ServiceProviderId.ToString(),
                    price
                );
                return true;
            }

            long noOfItemsAdded = await _cache.AddToListStringAsync(
                $"{string.Format(_keyTemplate, serviceId)}offersList",
                offerStr
            );

            if (noOfItemsAdded != 0)
            {
                await _cache.SetHashObjectAsync(
                    $"{string.Format(_keyTemplate, serviceId)}offersHash",
                    serviceProvider.ServiceProviderId.ToString(),
                    price
                );
                return true;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error communticating with cache server", ex);
        }

        return false;
    }

    public Task<decimal> GetOfferPriceAsync(int serviceId, int ServiceProviderId)
    {
        throw new NotImplementedException();
    }
}
