using ClientNexus.Application.Constants;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class EmergencyCaseService : IEmergencyCaseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOfferService _offerService;
    private readonly IServiceProviderService _serviceProviderService;
    private readonly IPushNotification _pushNotificationService;
    private readonly ICache _cache;

    public EmergencyCaseService(
        IUnitOfWork unitOfWork,
        IOfferService offerService,
        IServiceProviderService serviceProviderService,
        IPushNotification pushNotificationService,
        ICache cache
    )
    {
        _unitOfWork = unitOfWork;
        _offerService = offerService;
        _serviceProviderService = serviceProviderService;
        _pushNotificationService = pushNotificationService;
        _cache = cache;
    }

    private async Task<EmergencyCase> CreateEmergencyCaseAsync(
        CreateEmergencyCaseDTO emergencyDTO,
        int clientId
    )
    {
        EmergencyCase emergencyCase = new EmergencyCase
        {
            Name = emergencyDTO.Name,
            Description = emergencyDTO.Description,
            MeetingLatitude = emergencyDTO.MeetingLatitude,
            MeetingLongitude = emergencyDTO.MeetingLongitude,
            ClientId = clientId,
            Status = ServiceStatus.Pending,
        };

        await _unitOfWork.EmergencyCases.AddAsync(emergencyCase);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error creating emergency case", ex);
        }

        return emergencyCase;
    }

    public async Task<ClientEmergencyDTO> InitiateEmergencyCaseAsync(
        CreateEmergencyCaseDTO emergencyDTO,
        int clientId,
        string clientFirstName,
        string clientLastName,
        double notifyServicePorvidersWithinMeters = 3000,
        int allowOffersWithinMinutes = 16
    )
    {
        ArgumentNullException.ThrowIfNull(emergencyDTO);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientFirstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientLastName);

        EmergencyCase emergencyCase = await CreateEmergencyCaseAsync(emergencyDTO, clientId);
        bool offersAllowed = await _offerService.AllowOffersAsync(
            new ServiceProviderEmergencyDTO
            {
                ServiceId = emergencyCase.Id,
                ClientFirstName = clientFirstName,
                ClientLastName = clientLastName,
                Name = emergencyDTO.Name,
                Description = emergencyDTO.Description,
            },
            clientId,
            emergencyDTO.MeetingLongitude,
            emergencyDTO.MeetingLatitude,
            allowOffersWithinMinutes
        );

        if (!offersAllowed)
        {
            throw new Exception("Offers are already allowed for this emergency case.");
        }

        var providersTokens =
            await _serviceProviderService.GetTokensOfServiceProvidersNearLocationAsync(
                emergencyDTO.MeetingLongitude,
                emergencyDTO.MeetingLatitude,
                notifyServicePorvidersWithinMeters
            );

        foreach (var providerToken in providersTokens)
        {
            try
            {
                await _pushNotificationService.SendNotificationAsync(
                    $"New emergency case: {emergencyDTO.Name}",
                    $"Description: {emergencyDTO.Description}",
                    providerToken.Token
                );
            }
            catch (Exception) { }
        }

        return new ClientEmergencyDTO { Id = emergencyCase.Id };
    }

    public async Task<bool> CheckIfIdExistsAsync(int emergencyCaseId)
    {
        return await _unitOfWork.EmergencyCases.CheckAnyExistsAsync(ec => ec.Id == emergencyCaseId);
    }

    public async Task<bool> HasActiveEmergencyForClientAsync(int clientId)
    {
        var emergencyCase = (
            await _unitOfWork.EmergencyCases.GetByConditionAsync(
                ec =>
                    ec.CreatedAt >= DateTime.UtcNow.AddHours(-6)
                    && ec.ClientId == clientId
                    && ec.Status == ServiceStatus.InProgress,
                limit: 1
            )
        ).FirstOrDefault();

        return emergencyCase is not null;
    }

    public async Task<bool> HasActiveEmergencyForServiceProviderAsync(int serviceProviderId)
    {
        var emergencyCase = (
            await _unitOfWork.EmergencyCases.GetByConditionAsync(
                ec =>
                    ec.CreatedAt >= DateTime.UtcNow.AddHours(-6)
                    && ec.ServiceProviderId == serviceProviderId
                    && ec.Status == ServiceStatus.InProgress,
                limit: 1
            )
        ).FirstOrDefault();

        return emergencyCase is not null;
    }

    public async Task<bool> IsClientAllowedToCreateEmergencyAsync(int clientId)
    {
        var res = (
            await _unitOfWork.Clients.GetByConditionAsync(
                c => c.Id == clientId,
                c => new
                {
                    c.IsBlocked,
                    c.IsDeleted,
                    c.PhoneNumber,
                    c.NotificationToken,
                },
                limit: 1
            )
        ).FirstOrDefault();

        if (res is null)
        {
            throw new ArgumentException($"Client with {clientId} does not exist");
        }

        return !res.IsBlocked
            && !res.IsDeleted
            && res.PhoneNumber != null
            && res.NotificationToken != null
            && !await HasActiveEmergencyForClientAsync(clientId);
    }

    public async Task<(double longitude, double latitude)?> GetMeetingLocationAsync(
        int emergencyCaseId
    )
    {
        double? longitude = await _cache.GetObjectAsync<double?>(
            string.Format(CacheConstants.ServiceRequestLongitudeKeyTemplate, emergencyCaseId)
        );
        double? latitude = await _cache.GetObjectAsync<double?>(
            string.Format(CacheConstants.ServiceRequestLatitudeKeyTemplate, emergencyCaseId)
        );

        if (longitude is null || latitude is null)
        {
            return null;
        }

        return ((double)longitude, (double)latitude);
    }

    public async Task<(double longitude, double latitude)?> GetServiceProviderLocationAsync(
        int serviceProviderId
    )
    {
        var res = await _cache.GetGeoLocationAsync(
            CacheConstants.AvailableForEmergencyServiceProvidersLocationsKey,
            serviceProviderId.ToString()
        );

        if (res is null)
        {
            return null;
        }

        return (res.Longitude, res.Latitude);
    }

    public async Task<bool> SetServiceProviderLocationAsync(
        int serviceProviderId,
        double longitude,
        double latitude
    )
    {
        return await _cache.AddGeoLocationAsync(
            CacheConstants.AvailableForEmergencyServiceProvidersLocationsKey,
            longitude,
            latitude,
            serviceProviderId.ToString()
        );
    }
}
