using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Services
{
    public class SpecializationService : ISpecializationService
    {
        private readonly IUnitOfWork unitOfWork;

        public SpecializationService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task AddSpecializationsToServiceProvider(ICollection<ServiceProviderSpecialization> ServiceProviderSpecializations, List<int> SpecializationIDs, int serviceProviderId)
        {
            if (SpecializationIDs == null || !SpecializationIDs.Any())
            {
                throw new ArgumentNullException(nameof(SpecializationIDs), "Specialization IDs are required for ServiceProvider.");
            }
            //  Validate specialization IDs exist in the database
            var validSpecializationIds = (unitOfWork.Specializations.GetAllQueryable()).Select(s => s.Id).ToList();
            var invalidSpecializationIds = SpecializationIDs.Where(id => !validSpecializationIds.Contains(id)).ToList();
            if (invalidSpecializationIds.Any())
            {
                throw new ArgumentException($"Invalid Specialization IDs: {string.Join(", ", invalidSpecializationIds)}");
            }
            foreach (var specId in SpecializationIDs)
            {
                if (!ServiceProviderSpecializations.Any(s => s.SpecializationId == specId))
                {
                    ServiceProviderSpecializations.Add(new ServiceProviderSpecialization
                    {
                        SpecializationId = specId,
                        ServiceProviderId = serviceProviderId // Ensure the relationship is set correctly
                    });
                }
            }
            await unitOfWork.SaveChangesAsync();

        }

    }
}
