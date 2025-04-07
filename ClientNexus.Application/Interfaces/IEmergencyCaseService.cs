using ClientNexus.Application.DTO;

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
    Task<bool> ClientHasActiveEmergencyAsync(int clientId);
    Task<(double longitude, double latitude)?> GetMeetingLocationAsync(int emergencyCaseId);
    Task<(double longitude, double latitude)?> GetServiceProviderLocationAsync(
        int serviceProviderId
    );
    Task<bool> SetServiceProviderLocationAsync(
        int serviceProviderId,
        double longitude,
        double latitude
    );
}
