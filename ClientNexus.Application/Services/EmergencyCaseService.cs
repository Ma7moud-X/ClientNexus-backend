using ClientNexus.Application.Constants;
using ClientNexus.Application.DTO;
using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Domain.ValueObjects;
using NetTopologySuite.Geometries;

namespace ClientNexus.Application.Services;

public class EmergencyCaseService : IEmergencyCaseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceProviderService _serviceProviderService;
    private readonly IPushNotification _pushNotificationService;

    public EmergencyCaseService(
        IUnitOfWork unitOfWork,
        IServiceProviderService serviceProviderService,
        IPushNotification pushNotificationService
    )
    {
        _unitOfWork = unitOfWork;
        _serviceProviderService = serviceProviderService;
        _pushNotificationService = pushNotificationService;
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
            MeetingLocation = new MapPoint(
                emergencyDTO.MeetingLongitude,
                emergencyDTO.MeetingLatitude
            ),
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
        var providersTokens =
            await _serviceProviderService.GetTokensOfServiceProvidersNearLocationAsync(
                emergencyDTO.MeetingLongitude,
                emergencyDTO.MeetingLatitude,
                notifyServicePorvidersWithinMeters
            );

        foreach (var providerToken in providersTokens)
        {
            // Console.WriteLine($"Sending notification to {providerToken.Token}");
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

        return new ClientEmergencyDTO
        {
            Id = emergencyCase.Id,
            TimeoutInMinutes = allowOffersWithinMinutes - 1,
        };
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
                    ec.ClientId == clientId
                    && (
                        ec.CreatedAt >= DateTime.UtcNow.AddHours(-2)
                            && ec.Status == ServiceStatus.InProgress
                        || ec.CreatedAt.AddMinutes(15) >= DateTime.UtcNow
                            && ec.Status == ServiceStatus.Pending
                    ),
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
                    ec.CreatedAt >= DateTime.UtcNow.AddHours(-2)
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
            throw new ArgumentException($"Client with {clientId} does not exist");  // TODO: throw NotFoundException
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
        Point? location = (
            await _unitOfWork.EmergencyCases.GetByConditionAsync(
                ec => ec.Id == emergencyCaseId,
                ec => ec.MeetingLocation,
                limit: 1
            )
        ).FirstOrDefault();

        if (location is null)
        {
            return null;
        }

        return (location.X, location.Y);
    }

    public async Task<(double longitude, double latitude)?> GetServiceProviderLocationAsync(
        int serviceProviderId
    )
    {
        var location = (
            await _unitOfWork.ServiceProviders.GetByConditionAsync(
                sp => sp.Id == serviceProviderId,
                sp => sp.CurrentLocation,
                limit: 1
            )
        ).FirstOrDefault();

        if (location is null)
        {
            return null;
        }

        return (location.X, location.Y);
    }

    public async Task<bool> SetServiceProviderLocationAsync(
        int serviceProviderId,
        double longitude,
        double latitude
    )
    {
        var affectedCount = await _unitOfWork.SqlExecuteAsync(
            @"
            UPDATE ClientNexusSchema.ServiceProviders SET CurrentLocation = geography::Point(@latitude, @longitude, 4326), LastLocationUpdateTime = @lastLocationUpdateTime
            WHERE Id = @serviceProviderId
            ",
            new Parameter("@latitude", latitude),
            new Parameter("@longitude", longitude),
            new Parameter("@lastLocationUpdateTime", DateTime.UtcNow),
            new Parameter("@serviceProviderId", serviceProviderId)
        );

        return affectedCount != 0;
    }

    public async Task<EmergencyCaseOverviewDTO?> GetOverviewByIdAsync(int id)
    {
        return await _unitOfWork.SqlGetSingleAsync<EmergencyCaseOverviewDTO>(
            @"SELECT EmergencyCases.Id, Name AS Title, Description, Status, CreatedAt, Price, MeetingLongitude, MeetingLatitude, ClientId, ServiceProviderId
            FROM ClientNexusSchema.EmergencyCases
            JOIN ClientNexusSchema.Services ON EmergencyCases.Id = Services.Id
            WHERE EmergencyCases.Id = @id",
            new Parameter("@id", id)
        );
    }

    // public async Task<bool> CancelEmergencyCaseAsync(int id)
    // {
    //     int affectedCount = await _unitOfWork.SqlExecuteAsync(
    //         @$"
    //         UPDATE ClientNexusSchema.Services SET Status = '{(char)ServiceStatus.Cancelled}'
    //         WHERE Id in (
    //             SELECT Id FROM ClientNexusSchema.EmergencyCases WHERE Id = @id
    //         )
    //     ",
    //         new Parameter("@id", id)
    //     );
    // }
}
