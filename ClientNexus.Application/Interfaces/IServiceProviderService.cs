using ClientNexus.Application.Models;

namespace ClientNexus.Application.Interfaces;

public interface IServiceProviderService
{
    Task<ServiceProviderOverview> GetServiceProviderOverviewAsync(int serviceProviderId);
    Task<IEnumerable<NotificationToken>> GetTokensOfServiceProvidersNearLocationAsync(
        double longitude,
        double latitude,
        double radiusInMeters
    );
}
