using ClientNexus.Application.DTO;
using ClientNexus.Application.DTOs;

namespace ClientNexus.Application.Interfaces;

public interface IEmergencyCaseService
{
    Task<ClientEmergencyDTO> InitiateEmergencyCaseAsync(
        CreateEmergencyCaseDTO emergencyDTO,
        int clientId,
        string clientFirstName,
        string clientLastName,
        double notifyServicePorvidersWithinMeters = 3000,
        int allowOffersWithinMinutes = 16
    );

    Task<bool> CheckIfIdExistsAsync(int emergencyCaseId);
    Task<bool> HasActiveEmergencyForClientAsync(int clientId);
    Task<(double longitude, double latitude)?> GetMeetingLocationAsync(int emergencyCaseId);
    Task<(double longitude, double latitude)?> GetServiceProviderLocationAsync(
        int serviceProviderId
    );
    Task<bool> SetServiceProviderLocationAsync(
        int serviceProviderId,
        double longitude,
        double latitude
    );

    Task<bool> HasActiveEmergencyForServiceProviderAsync(int serviceProviderId);
    Task<bool> IsClientAllowedToCreateEmergencyAsync(int clientId);
    Task<EmergencyCaseOverviewDTO?> GetOverviewByIdAsync(int id);
}
