using ClientNexus.Application.Models;

namespace ClientNexus.Application.Interfaces;

public interface IServiceProviderService
{
    Task<ServiceProviderOverview?> GetServiceProviderOverviewAsync(int serviceProviderId);
    Task<IEnumerable<NotificationToken>> GetTokensOfServiceProvidersNearLocationAsync(
        double longitude,
        double latitude,
        double radiusInMeters
    );
    Task<bool> SetUnvavailableForEmergencyAsync(int serviceProviderId);
    Task<bool> CheckIfAllowedToMakeOffersAsync(int serviceProviderId);
    Task<bool> SetAvailableForEmergencyAsync(int serviceProviderId);
    // Task NotifyAboutOfferAcceptanceAsync(
    //     int serviceProviderId,
    //     int serviceId,
    //     string clientPhoneNumber
    // );
}
