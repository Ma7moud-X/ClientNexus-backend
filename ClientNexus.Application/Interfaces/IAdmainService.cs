using ClientNexus.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IAdmainService
    {
        public Task ApprovingServiceProviderAsync(int ServiceID, int adminId);
        //public Task AddCountryAsync(CountryDTO countryDTO);
        //public Task DeleteCountryAsync(int id);
        //public Task AddStateAsync(StateDTO stateDTO);
        //public Task DeleteStateAsync(int id);
        //public Task AddCityAsync(CityDTO cityDTO);
        //public Task DeleteCityAsync(int id);
        //public Task AddServiceProviderTypeAsyn(string Name);
        //public Task DeleteServiceProviderTypeAsync(int id);
        //public Task AddSpecializationAsync(SpecializationDTO specializationDTO);
        //public Task DeleteSpecializationAsync(int id);

    }
}
