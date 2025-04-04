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
        int allowOffersWithinMinutes = 160
    );
}
