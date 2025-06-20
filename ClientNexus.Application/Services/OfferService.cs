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
    private readonly IEmergencyCaseService _emergencyCaseService;
    private readonly INotificationService _notificationService;

    public OfferService(
        ICache cache,
        IEventPublisher eventPublisher,
        IUnitOfWork unitOfWork,
        IBaseServiceService baseServiceService,
        IServiceProviderService serviceProviderService,
        IEmergencyCaseService emergencyCaseService,
        INotificationService notificationService
    )
    {
        _cache = cache;
        _eventPublisher = eventPublisher;
        _unitOfWork = unitOfWork;
        _baseServiceService = baseServiceService;
        _serviceProviderService = serviceProviderService;
        _emergencyCaseService = emergencyCaseService;
        _notificationService = notificationService;
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
            throw new NotFoundException("Emergency case not found");
        }

        TimeSpan requestTTL =
            createdAt.Value.AddMinutes(GlobalConstants.EmergencyCaseTTLInMinutes) - DateTime.UtcNow;

        if (requestTTL.TotalMinutes <= 1)
        {
            throw new ExpiredException("Request is no longer accepting offers");
        }

        _cache.StartTransaction();

        var offerSet = _cache.SetObjectAsync(
            string.Format(
                CacheConstants.ServiceOfferPriceKeyTemplate,
                serviceId,
                serviceProvider.ServiceProviderId
            ),
            price,
            offerTTL < requestTTL ? offerTTL : requestTTL,
            @override: false
        );

        var activeOfferSet = _cache.SetStringAsync(
            string.Format(
                CacheConstants.ServiceProviderHasActiveOfferKeyTemplate,
                serviceProvider.ServiceProviderId
            ),
            "1",
            offerTTL
        );

        bool commited = await _cache.CommitTransactionAsync();
        if (!commited)
        {
            throw new ServerException("Error creating an offer. Please try again later.");
        }

        if (!await offerSet)
        {
            throw new NotAllowedException(
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
            throw new ServerException(
                "Error creating the offer. Transaction saving missed offers has failed"
            );
        }

        if (await noOfItemsAdded != 0)
        {
            return;
        }

        throw new ServerException("Error publishing offer");
    }

    public async Task<decimal?> GetOfferPriceAsync(int serviceId, int serviceProviderId)
    {
        return await _cache.GetObjectAsync<decimal?>(
            string.Format(CacheConstants.ServiceOfferPriceKeyTemplate, serviceId, serviceProviderId)
        );
    }

    public async Task<bool> HasActiveOfferAsync(int serviceProviderId)
    {
        var res = await _cache.GetStringAsync(
            string.Format(
                CacheConstants.ServiceProviderHasActiveOfferKeyTemplate,
                serviceProviderId
            )
        );

        return res is not null;
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
            throw new ExpiredException("Offer has expired or does not exist");
        }

        var serviceStatusEnumerable = await _unitOfWork.Services.GetByConditionAsync(
            s => s.Id == serviceId,
            s => s.Status
        );

        if (serviceStatusEnumerable is null || !serviceStatusEnumerable.Any())
        {
            throw new NotFoundException("Service does not exist");
        }

        var serviceStatus = serviceStatusEnumerable.FirstOrDefault();
        if (serviceStatus != ServiceStatus.Pending)
        {
            throw new NotAllowedException("Service can't accept offers");
        }

        await _unitOfWork.BeginTransactionAsync();

        bool unAvailableSet = false;
        try
        {
            unAvailableSet =
                await _serviceProviderService.SetUnvavailableForEmergencyWithLockingAsync(
                    serviceProviderId
                );
        }
        catch (NotFoundException)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        if (!unAvailableSet)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new NotAllowedException("Service provider is no longer available");
        }

        var assigned = await _baseServiceService.AssignServiceProviderAsync(
            serviceId,
            clientId,
            serviceProviderId,
            price.Value
        );

        if (!assigned)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new ServerException("Failed to assign service provider to service");
        }

        var result = (
            await _unitOfWork.BaseUsers.GetByConditionAsync(
                u => u.Id == serviceProviderId || u.Id == clientId,
                u => new { u.Id, u.PhoneNumber }
            )
        ).ToList();

        var meetingLocation = await _emergencyCaseService.GetMeetingLocationAsync(serviceId);

        var providerDetails = result.FirstOrDefault(u => u.Id == serviceProviderId);
        var clientDetails = result.FirstOrDefault(u => u.Id == clientId);

        if (providerDetails is null || providerDetails.PhoneNumber is null) // can't happen
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new ServerException("Service provider info is missing");
        }

        if (clientDetails is null || clientDetails.PhoneNumber is null) // can't happen
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new ServerException("Client info is missing");
        }

        await _unitOfWork.CommitTransactionAsync();

        string notificationTitle = "Offer accepted";
        string notificationBody =
            $"Client Phone Number: {clientDetails.PhoneNumber}";

        bool isSent = await _notificationService.SendNotificationAsync(
            notificationTitle,
            notificationBody,
            serviceProviderId
        );

        var cachedOffersRemovedTask = _cache.RemoveKeyAsync(
            string.Format(CacheConstants.MissedOffersKeyTemplate, serviceId)
        );

        return new PhoneNumberDTO { PhoneNumber = providerDetails.PhoneNumber };
    }
}
