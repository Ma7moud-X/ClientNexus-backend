using ClientNexus.Application.DTOs;
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
    Task<bool> CheckIfAllowedToBeAvailableForEmergencyAsync(int serviceProviderId);
    Task<bool> SetAvailableForEmergencyAsync(int serviceProviderId);
    public Task<List<ServiceProviderResponse>> SearchServiceProvidersAsync(string? searchQuery);
    public Task UpdateServiceProviderAsync(
        int ServiceProviderId,
        UpdateServiceProviderDTO updateDto
    );
    public Task<List<ServiceProviderResponse>> FilterServiceProviderResponses(
        string searchQuery,
        float? minRate,
        string? state,
        string? city,
        string? specializationName
    );
}
