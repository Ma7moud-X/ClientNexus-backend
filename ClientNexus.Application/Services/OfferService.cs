using System.Text.Json;
using ClientNexus.Application.Constants;
using ClientNexus.Application.Domain;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Models;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class OfferService : IOfferService
{
    private readonly ICache _cache;
    private readonly IEventPublisher _eventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public OfferService(ICache cache, IEventPublisher eventPublisher, IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(eventPublisher);

        _cache = cache;
        _eventPublisher = eventPublisher;
        _unitOfWork = unitOfWork;
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

        // _cache.StartTransaction();

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

        // var hashCreated = _cache.SetHashStringAsync(
        //     $"{string.Format(_keyTemplate, service.ServiceId)}offersHash",
        //     "empty",
        //     "empty"
        // );
        // var hashEmptied = _cache.RemoveHashFieldAsync("empty", "empty");
        // var hashExpirySet = _cache.SetExpiryAsync(
        //     $"{string.Format(_keyTemplate, service.ServiceId)}offersHash",
        //     TimeSpan.FromMinutes(timeoutInMin)
        // );

        // bool transactionCommitted;
        // try
        // {
        //     transactionCommitted = await _cache.CommitTransactionAsync();
        // }
        // catch (Exception ex)
        // {
        //     throw new Exception("Error committing transaction", ex);
        // }

        var created = await _cache.SetObjectAsync(
            string.Format(CacheConstants.ServiceRequestKeyTemplate, service.ServiceId),
            service,
            TimeSpan.FromMinutes(timeoutInMin),
            @override: false
        );

        return created;
    }

    public async Task CreateOfferAsync(
        int serviceId,
        decimal price,
        ServiceProviderOverview serviceProvider,
        TravelDistance travelDistance,
        TimeSpan offerTTL
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
                ExpiresAt = DateTime.UtcNow.Add(offerTTL),
            }
        );

        TimeSpan? requestTTL = await _cache.GetTTLAsync(
            string.Format(CacheConstants.ServiceRequestKeyTemplate, serviceId)
        );

        if (requestTTL is null || requestTTL.Value.TotalMinutes <= 1)
        {
            throw new Exception("Request is no longer accepting offers");
        }

        // _cache.StartTransaction();
        // var hashSet = _cache.SetHashObjectAsync(
        //     string.Format(CacheConstants.OffersHashKeyTemplate, serviceId),
        //     serviceProvider.ServiceProviderId.ToString(),
        //     price,
        //     @override: false
        // );
        // var expirySet = _cache.SetExpiryAsync(
        //     string.Format(CacheConstants.OffersHashKeyTemplate, serviceId),
        //     offerTTL < requestTTL ? offerTTL : requestTTL.Value
        // );
        // var transactionCommitted1 = await _cache.CommitTransactionAsync();

        // if (!transactionCommitted1)
        // {
        //     throw new Exception(
        //         "Error creating the offer. Transaction saving offer price has failed"
        //     );
        // }

        // if (!await hashSet)
        // {
        //     throw new Exception(
        //         "Can't make another offer without waiting for previous offer to expire"
        //     );
        // }

        var offerSet = await _cache.SetObjectAsync(
            string.Format(
                CacheConstants.ServiceOfferPriceKeyTemplate,
                serviceId,
                serviceProvider.ServiceProviderId
            ),
            price,
            offerTTL < requestTTL ? offerTTL : requestTTL.Value,
            @override: false
        );

        if (!offerSet)
        {
            throw new Exception(
                "Can't make another offer without waiting for previous offer to expire"
            );
        }

        long receivedByCount = await _eventPublisher.PublishAsync(
            string.Format(CacheConstants.OffersChannelKeyTemplate, serviceId),
            offerStr
        );

        if (receivedByCount != 0)
        {
            return;
        }

        _cache.StartTransaction();
        var noOfItemsAdded = _cache.AddToListStringAsync(
            string.Format(CacheConstants.MissedOffersKeyTemplate, serviceId),
            offerStr
        );
        _ = _cache.SetExpiryAsync(
            string.Format(CacheConstants.MissedOffersKeyTemplate, serviceId),
            offerTTL < requestTTL ? offerTTL : requestTTL.Value
        );
        var transactionCommitted2 = await _cache.CommitTransactionAsync();

        if (!transactionCommitted2)
        {
            throw new Exception(
                "Error creating the offer. Transaction saving missed offers has failed"
            );
        }

        if (await noOfItemsAdded != 0)
        {
            return;
        }

        throw new Exception("Error publishing offer");
    }

    public async Task<decimal?> GetOfferPriceAsync(int serviceId, int serviceProviderId)
    {
        return await _cache.GetObjectAsync<decimal?>(
            string.Format(CacheConstants.ServiceOfferPriceKeyTemplate, serviceId, serviceProviderId)
        );
    }

    public async Task AcceptOfferAsync(int serviceId, int serviceProviderId)
    {
        var price = await GetOfferPriceAsync(serviceId, serviceProviderId);
        if (price is null)
        {
            throw new Exception("Offer has expired");
        }

        var serviceStatusEnumerable = await _unitOfWork.Services.GetByConditionAsync(
            s => s.Id == serviceId,
            s => s.Status
        );

        if (serviceStatusEnumerable is null || !serviceStatusEnumerable.Any())
        {
            throw new Exception("Service does not exist");
        }

        var serviceStatus = serviceStatusEnumerable.FirstOrDefault();
        if (serviceStatus != ServiceStatus.Pending)
        {
            throw new Exception("Service is not in a pending state");
        }

        throw new NotImplementedException();    // TODO: Implement the logic to accept the offer

        int affectedCount = await _unitOfWork.SqlExecuteAsync(
            @"
            


            UPDATE ClientNexusSchema.Services SET Status = 'I', Price = @price, ServiceProviderId = @serviceProviderId
            WHERE Id = @serviceId;
        ",
            new Parameter("@price", price),
            new Parameter("@serviceProviderId", serviceProviderId),
            new Parameter("@serviceId", serviceId)
        );
    }
}
