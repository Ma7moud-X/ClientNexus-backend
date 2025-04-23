
using ClientNexus.Application.DTOs;
using ClientNexus.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface ISpecializationService
    {
        public Task AddSpecializationsToServiceProvider(ICollection<ServiceProviderSpecialization> ServiceProviderSpecializations, List<int> SpecializationIDs, int serviceProviderId);
        public Task AddNewSpecializationAsync(SpecializationDTO specializationDTO);
        public  Task DeleteSpecializationAsync(int id);


    }
}
