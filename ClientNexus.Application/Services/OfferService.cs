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
    private readonly IBaseServiceService _baseServiceService;
    private readonly IServiceProviderService _serviceProviderService;

    public OfferService(
        ICache cache,
        IEventPublisher eventPublisher,
        IUnitOfWork unitOfWork,
        IBaseServiceService baseServiceService,
        IServiceProviderService serviceProviderService
    )
    {
        _cache = cache;
        _eventPublisher = eventPublisher;
        _unitOfWork = unitOfWork;
        _baseServiceService = baseServiceService;
        _serviceProviderService = serviceProviderService;
    }

    public async Task<bool> AllowOffersAsync<T>(
        T service,
        int clientId,
        double MeetingLongitude,
        double MeetingLatitude,
        int timeoutInMin = 16
    )
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

        var requestCached = _cache.SetObjectAsync(
            string.Format(CacheConstants.ServiceRequestKeyTemplate, service.ServiceId),
            service,
            TimeSpan.FromMinutes(timeoutInMin)
        );

        var longitudeCached = _cache.SetObjectAsync(
            string.Format(CacheConstants.ServiceRequestLongitudeKeyTemplate, service.ServiceId),
            MeetingLongitude,
            TimeSpan.FromMinutes(timeoutInMin)
        );

        var latitudeCached = _cache.SetObjectAsync(
            string.Format(CacheConstants.ServiceRequestLatitudeKeyTemplate, service.ServiceId),
            MeetingLatitude,
            TimeSpan.FromMinutes(timeoutInMin)
        );

        // var hasActiveRequestCached = _cache.SetObjectAsync(
        //     string.Format(CacheConstants.ClientHasActiveEmergencyRequestKeyTemplate, clientId),
        //     service.ServiceId,
        //     TimeSpan.FromMinutes(timeoutInMin)
        // );

        var transactionCommitted = await _cache.CommitTransactionAsync();
        if (!transactionCommitted)
        {
            throw new Exception("Error creating the service request.");
        }

        return await requestCached;
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

    public async Task AcceptOfferAsync(int serviceId, int clientId, int serviceProviderId)
    {
        var price = await GetOfferPriceAsync(serviceId, serviceProviderId);
        if (price is null)
        {
            throw new Exception("Offer has expired or does not exist");
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
            throw new Exception("Service can't accept offers");
        }

        bool unAvailableSet = await _serviceProviderService.SetUnvavailableForEmergencyAsync(
            serviceProviderId
        );
        if (!unAvailableSet)
        {
            throw new Exception("Service provider is no longer available");
        }

        var assigned = await _baseServiceService.AssignServiceProviderAsync(
            serviceId,
            clientId,
            serviceProviderId,
            price.Value
        );

        if (!assigned)
        {
            throw new Exception("Failed to assign service provider to service");
        }

        _cache.StartTransaction();

        var requestRemoved = _cache.RemoveKeyAsync(
            string.Format(CacheConstants.ServiceRequestKeyTemplate, serviceId)
        );
        var cachedOffersRemoved = _cache.RemoveKeyAsync(
            string.Format(CacheConstants.MissedOffersKeyTemplate, serviceId)
        );
        var longitudeRmoved = _cache.RemoveKeyAsync(
            string.Format(CacheConstants.ServiceRequestLongitudeKeyTemplate, serviceId)
        );
        var latitudeRmoved = _cache.RemoveKeyAsync(
            string.Format(CacheConstants.ServiceRequestLatitudeKeyTemplate, serviceId)
        );
        
        var commited = _cache.CommitTransactionAsync();

        // TODO: Add logic of notifying the servAice provider about the offer acceptance
    }
}
