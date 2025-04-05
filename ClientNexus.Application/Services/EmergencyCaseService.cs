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

    public EmergencyCaseService(
        IUnitOfWork unitOfWork,
        IOfferService offerService,
        IServiceProviderService serviceProviderService,
        IPushNotification pushNotificationService
    )
    {
        _unitOfWork = unitOfWork;
        _offerService = offerService;
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
        int allowOffersWithinMinutes = 160
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
                MeetingLatitude = emergencyDTO.MeetingLatitude,
                MeetingLongitude = emergencyDTO.MeetingLongitude,
            },
            allowOffersWithinMinutes
        );

        if (!offersAllowed)
        {
            throw new Exception("Error while allowing offers");
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

    public async Task CancelEmergencyCaseAsync(int emergencyId)
    {
        var affectedCount = await _unitOfWork.SqlExecuteAsync(
            @"
            UPDATE ClientNexusSchema.Services SET Status = 'C' WHERE Id IN (
                SELECT Services.Id FROM ClientNexusSchema.Services, ClientNexusSchema.EmergencyCases
                WHERE ClientNexusSchema.Services.Id = ClientNexusSchema.EmergencyCases.Id
                    AND ClientNexusSchema.EmergencyCases.Id = @emergencyId
                    AND ClientNexusSchema.Services.Status = 'P')
        ",
            new Parameter("@emergencyId", emergencyId)
        );

        if (affectedCount == 0)
        {
            throw new InvalidOperationException(
                "Invalid operation. Emergency case does not exist or isn't pending."
            );
        }
    }

    // public async Task<bool> AssignServiceProviderAsync(
    //     int emergencyCaseId,
    //     int clientID,
    //     int serviceProviderId,
    //     decimal price
    // )
    // {
    //     var emergencyCase = (
    //         await _unitOfWork.EmergencyCases.GetByConditionAsync(
    //             condExp: ec => ec.Id == emergencyCaseId && ec.ClientId == clientID,
    //             selectExp: sp => new { sp.CreatedAt, sp.ServiceProviderId }
    //         )
    //     ).FirstOrDefault();

    //     bool serviceProviderExists = await _unitOfWork.ServiceProviders.CheckAnyExistsAsync(sp =>
    //         sp.Id == serviceProviderId
    //     );
    //     if (!serviceProviderExists)
    //     {
    //         throw new ArgumentException(
    //             "Invalid service provider ID. Service provider does not exist.",
    //             nameof(serviceProviderId)
    //         );
    //     }
    // }
}
