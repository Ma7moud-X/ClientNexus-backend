using ClientNexus.Application.Constants;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Models;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICache _cache;
        private readonly IBaseUserService _baseUserService;

        public ServiceProviderService(
            IUnitOfWork unitOfWork,
            ICache cache,
            IBaseUserService baseUserService
        )
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _baseUserService = baseUserService;
        }

        public Task<ServiceProviderOverview> GetServiceProviderOverviewAsync(int serviceProviderId) // TODO: to be implemented
        {
            throw new NotImplementedException();
        }

        public async Task<
            IEnumerable<NotificationToken>
        > GetTokensOfServiceProvidersNearLocationAsync(
            double longitude,
            double latitude,
            double radiusInMeters
        )
        {
            var providersLocations = await _cache.GetGeoLocationsInRadiusAsync(
                CacheConstants.AvailableForEmergencyServiceProvidersLocationsKey,
                longitude,
                latitude,
                radiusInMeters
            );

            if (providersLocations is null || !providersLocations.Any())
            {
                return Enumerable.Empty<NotificationToken>();
            }

            IEnumerable<int> providersIds;
            try
            {
                providersIds = providersLocations.Select(l => int.Parse(l.Identifier));
            }
            catch (Exception)
            {
                throw new InvalidCastException(
                    "Error casting service provider IDs from cache locations as integers"
                );
            }

            return await _baseUserService.GetNotificationTokensAsync(providersIds);
        }
    }
}
