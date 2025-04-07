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

        public async Task<bool> CheckIfAllowedToMakeOffersAsync(int serviceProviderId)
        {
            var res = (
                await _unitOfWork.ServiceProviders.GetByConditionAsync(
                    sp => sp.Id == serviceProviderId,
                    sp => new
                    {
                        sp.IsAvailableForEmergency,
                        sp.ApprovedById,
                        sp.BlockedById,
                    }
                )
            ).FirstOrDefault();

            if (res is null)
            {
                throw new ArgumentException(
                    $"Service provider with ID {serviceProviderId} not found"
                );
            }

            return res.IsAvailableForEmergency
                && res.ApprovedById != null
                && res.BlockedById == null;
        }

        public async Task<ServiceProviderOverview?> GetServiceProviderOverviewAsync(
            int serviceProviderId
        ) // TODO: to be implemented
        {
            return await _unitOfWork.SqlGetSingleAsync<ServiceProviderOverview>(
                @"
                select ServiceProviders.Id as ServiceProviderId, FirstName, LastName, YearsOfExperience, MainImage as ImageUrl, Rate as Rating
                from ClientNexusSchema.ServiceProviders join ClientNexusSchema.BaseUsers
                on BaseUsers.Id = ServiceProviders.Id
                where ServiceProviders.Id = @serviceProviderId
                ",
                new Parameter("@serviceProviderId", serviceProviderId)
            );
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

        public async Task<bool> SetAvailableForEmergencyAsync(int serviceProviderId)
        {
            var res = (
                await _unitOfWork.ServiceProviders.GetByConditionAsync(
                    sp => sp.Id == serviceProviderId,
                    sp => new { sp.ApprovedById, sp.BlockedById }
                )
            ).FirstOrDefault();

            if (res is null)
            {
                throw new ArgumentException(
                    $"Service provider with ID {serviceProviderId} not found"
                );
            }

            if (res.ApprovedById == null || res.BlockedById != null)
            {
                return false;
            }

            int affectedCount = await _unitOfWork.SqlExecuteAsync(
                @"
                UPDATE ClientNexusSchema.ServiceProviders SET IsAvailableForEmergency = 1
                WHERE Id = @serviceProviderId;
                ",
                new Parameter("@serviceProviderId", serviceProviderId)
            );

            return affectedCount == 1;
        }

        public async Task<bool> SetUnvavailableForEmergencyAsync(int serviceProviderId)
        {
            await _unitOfWork.BeginTransactionAsync();

            var availability = await _unitOfWork.SqlGetSingleAsync<AvailableForEmergencyResponse>(
                @"
                SELECT IsAvailableForEmergency FROM ClientNexusSchema.ServiceProviders
                WITH (UPDLOCK, HOLDLOCK)
                WHERE Id = @serviceProviderId
                ",
                new Parameter("@serviceProviderId", serviceProviderId)
            );

            if (availability is null || availability.IsAvailableForEmergency == false)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return false;
            }

            var affectedCount = await _unitOfWork.SqlExecuteAsync(
                @"
                UPDATE ClientNexusSchema.ServiceProviders SET IsAvailableForEmergency = 0
                WHERE Id = @serviceProviderId;
            ",
                new Parameter("@serviceProviderId", serviceProviderId)
            );

            await _unitOfWork.CommitTransactionAsync();

            return affectedCount == 1;
        }
    }
}
