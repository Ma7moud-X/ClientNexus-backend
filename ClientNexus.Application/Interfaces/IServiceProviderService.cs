using ClientNexus.Application.DTOs;
using ClientNexus.Application.Models;

namespace ClientNexus.Application.Interfaces
{
    public interface IServiceProviderService
    {
        Task<ServiceProviderOverview?> GetServiceProviderOverviewAsync(int serviceProviderId);

        Task<bool> SetUnvavailableForEmergencyWithLockingAsync(int serviceProviderId);
        Task<bool> CheckIfAllowedToMakeOffersAsync(int serviceProviderId);
        Task<bool> CheckIfAllowedToBeAvailableForEmergencyAsync(int serviceProviderId);
        Task<bool> SetAvailableForEmergencyAsync(int serviceProviderId);
        public Task<List<ServiceProviderResponseDTO>> SearchServiceProvidersAsync(
            string? searchQuery
        );
        public Task UpdateServiceProviderAsync(
            int ServiceProviderId,
            UpdateServiceProviderDTO updateDto
        );
        public Task<List<ServiceProviderResponseDTO>> FilterServiceProviderResponses(
            string searchQuery,
            float? minRate,
            string? state,
            string? city,
            string? specializationName
        );
        public Task<List<ServiceProviderResponseDTO>> GetAllServiceProvider(bool? IsApproved);
        public Task<ServiceProviderResponseDTO> GetByIdAsync(int ServiceProviderId);
        public Task UpdateServiceProviderPasswordAsync(int ServiceProviderId, UpdatePasswordDTO dto);

    }
}
