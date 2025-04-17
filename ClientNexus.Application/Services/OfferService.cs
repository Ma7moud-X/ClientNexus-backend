using System.Text.Json;
using ClientNexus.Application.Constants;
using ClientNexus.Application.Domain;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Models;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class OfferService : IOfferService
{
    private readonly ICache _cache;
    private readonly IEventPublisher _eventPublisher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseServiceService _baseServiceService;
    private readonly IServiceProviderService _serviceProviderService;
    private readonly IPushNotification _pushNotificationService;
    private readonly IEmergencyCaseService _emergencyCaseService;

    public OfferService(
        ICache cache,
        IEventPublisher eventPublisher,
        IUnitOfWork unitOfWork,
        IBaseServiceService baseServiceService,
        IServiceProviderService serviceProviderService,
        IPushNotification pushNotificationService,
        IEmergencyCaseService emergencyCaseService
    )
    {
        _cache = cache;
        _eventPublisher = eventPublisher;
        _unitOfWork = unitOfWork;
        _baseServiceService = baseServiceService;
        _serviceProviderService = serviceProviderService;
        _pushNotificationService = pushNotificationService;
        _emergencyCaseService = emergencyCaseService;
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

        DateTime? createdAt = (
            await _unitOfWork.EmergencyCases.GetByConditionAsync(
                ec => ec.Id == serviceId,
                ec => ec.CreatedAt
            )
        ).FirstOrDefault();
        if (createdAt is null)
        {
            throw new InvalidOperationException("Emergency case not found");
        }

        TimeSpan requestTTL =
            createdAt.Value.AddMinutes(GlobalConstants.EmergencyCaseTTL) - createdAt.Value;

        if (requestTTL.TotalMinutes <= 1)
        {
            throw new InvalidOperationException("Request is no longer accepting offers");
        }

        var offerSet = await _cache.SetObjectAsync(
            string.Format(
                CacheConstants.ServiceOfferPriceKeyTemplate,
                serviceId,
                serviceProvider.ServiceProviderId
            ),
            price,
            offerTTL < requestTTL ? offerTTL : requestTTL,
            @override: false
        );

        if (!offerSet)
        {
            throw new InvalidOperationException(
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
            offerTTL < requestTTL ? offerTTL : requestTTL
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

    public async Task<PhoneNumberDTO> AcceptOfferAsync(
        int serviceId,
        int clientId,
        int serviceProviderId
    )
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

        var cachedOffersRemoved = await _cache.RemoveKeyAsync(
            string.Format(CacheConstants.MissedOffersKeyTemplate, serviceId)
        );

        var providerDetails = (
            await _unitOfWork.ServiceProviders.GetByConditionAsync(
                sp => sp.Id == serviceProviderId,
                sp => new { sp.NotificationToken, sp.PhoneNumber }
            )
        ).FirstOrDefault();

        if (providerDetails is null)
        {
            throw new Exception("Service provider does not exist");
        }

        if (providerDetails.NotificationToken is null || providerDetails.PhoneNumber is null)
        {
            throw new ServerException(
                "Service provider notification token or phone number does not exist"
            );
        }

        string? clientPhoneNumber = (
            await _unitOfWork.Clients.GetByConditionAsync(c => c.Id == clientId, c => c.PhoneNumber)
        ).FirstOrDefault();

        if (clientPhoneNumber is null)
        {
            throw new ServerException("Client phone number does not exist");
        }

        var meetingLocation = await _emergencyCaseService.GetMeetingLocationAsync(serviceId);
        await _pushNotificationService.SendNotificationAsync(
            "Offer accepted",
            $"Meeting Longitude: {meetingLocation.Value.longitude}\nMeeting Latitude: {meetingLocation.Value.latitude}\nClient Phone Number: {clientPhoneNumber}\nService Id: {serviceId}",
            providerDetails.NotificationToken
        );

        return new PhoneNumberDTO { PhoneNumber = providerDetails.PhoneNumber };
    }
}
