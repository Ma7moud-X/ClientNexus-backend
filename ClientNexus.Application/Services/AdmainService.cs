using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Services
{
    public class AdmainService : IAdmainService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<BaseUser> _userManager;
        public AdmainService(IUnitOfWork unitOfWork, UserManager<BaseUser> userManager)

        {
            this.unitOfWork = unitOfWork;
            this._userManager = userManager;
        }
        public async Task ApprovingServiceProviderAsync(int ServiceID)
        {
            // Retrieve the service provider from the database
            var serviceProvider = await unitOfWork.ServiceProviders.GetByIdAsync(ServiceID);

            // Check if service provider exists
            if (serviceProvider == null)
            {
                throw new KeyNotFoundException($"Service provider with ID {ServiceID} not found.");
            }

            serviceProvider.IsApproved = true;
            await unitOfWork.SaveChangesAsync();

        }
        public async Task AddCountryAsync(CountryDTO countryDTO)
        {
            if (countryDTO == null)
            {
                throw new ArgumentException("Country name is required.", nameof(countryDTO));
            }
            // Check if the Country already exists

            var countries = unitOfWork.Countries.GetAllQueryable();
            var existingCountry = countries.Any(s => s.Name.Trim().ToLower() == countryDTO.Name.Trim().ToLower());
            if (existingCountry)
            {
                throw new InvalidOperationException($"Country '{countryDTO.Name}' already exists.");

            }
            var country = new Country
            {
                Name = countryDTO.Name,
                Abbreviation = countryDTO.Abbreviation
            };
            await unitOfWork.Countries.AddAsync(country);
            await unitOfWork.SaveChangesAsync();

        }

        public async Task DeleteCountryAsync(int id)
        {
            var Country = await unitOfWork.Countries.GetByIdAsync(id);
            if (Country == null)
            {
                throw new KeyNotFoundException("Country not found.");

            }

            unitOfWork.Countries.Delete(Country);
            await unitOfWork.SaveChangesAsync();
        }
        public async Task AddStateAsync(StateDTO stateDTO)
        {
            if (stateDTO == null)
            {
                throw new ArgumentException("state name is required.", nameof(stateDTO));
            }
            var countries = unitOfWork.Countries.GetAllQueryable();
            var defaultCountry = countries.FirstOrDefault(c => c.Name.Trim().Equals("مصر", StringComparison.OrdinalIgnoreCase));

            if (defaultCountry == null)
            {
                throw new Exception("Default country (Egypt) not found in the database.");
            }
            // Check if the state already exists
            var existingStates = unitOfWork.States.GetAllQueryable();
            bool stateExists = existingStates.Any(s =>
                s.Name.Trim().ToLower() == stateDTO.Name.Trim().ToLower() && s.CountryId == defaultCountry.Id
            );
            if (stateExists)
            {
                throw new InvalidOperationException($"State '{stateDTO.Name}' already exists in Egypt.");
            }
            var state = new State
            {
                Name = stateDTO.Name,
                Abbreviation = stateDTO.Abbreviation,
                CountryId = defaultCountry.Id // Assign default country
            };
            await unitOfWork.States.AddAsync(state);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteStateAsync(int id)
        {
            var state = await unitOfWork.States.GetByIdAsync(id);
            if (state == null)
            {
                throw new KeyNotFoundException("state not found.");

            }

            unitOfWork.States.Delete(state);
            await unitOfWork.SaveChangesAsync();
        }
        public async Task AddCityAsync(CityDTO cityDTO)
        {
            if (cityDTO == null)
            {
                throw new ArgumentException("city data is required.", nameof(cityDTO));
            }

            // Validate StateId (it must exist in the database)
            var state = (unitOfWork.States.GetAllQueryable())
                .FirstOrDefault(s => s.Id == cityDTO.StateId);

            if (state == null)
            {
                throw new ArgumentException("The provided StateId does not exist in the database.", nameof(cityDTO.StateId));
            }

            // Check if the city already exists in the same state
            bool cityExists = (unitOfWork.Cities.GetAllQueryable().Any(c => c.Name.Trim().ToLower() == cityDTO.Name.Trim().ToLower() && c.StateId == cityDTO.StateId));
            if (cityExists)
            {
                throw new InvalidOperationException($"City '{cityDTO.Name}' already exists in the selected state.");
            }

            var city = new City
            {
                Name = cityDTO.Name,
                Abbreviation = cityDTO.Abbreviation,
                StateId = cityDTO.StateId,
                CountryId = 1

            };
            await unitOfWork.Cities.AddAsync(city);
            await unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteCityAsync(int id)
        {
            var city = await unitOfWork.Cities.GetByIdAsync(id);
            if (city == null)
            {
                throw new KeyNotFoundException("City not found.");

            }

            unitOfWork.Cities.Delete(city);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task AddServiceProviderTypeAsyn(string Name)
        {
            // Check if a ServiceProviderType with the same name already exists
            bool exists = unitOfWork.ServiceProviderTypes.GetAllQueryable().Any(spt => spt.Name == Name);
            if (exists)
            {
                throw new InvalidOperationException($"ServiceProviderType '{Name}' already exists.");

            }
            ServiceProviderType serviceProviderType = new ServiceProviderType()
            {
                Name = Name
            };
            await unitOfWork.ServiceProviderTypes.AddAsync(serviceProviderType);
            await unitOfWork.SaveChangesAsync();


        }
        public async Task DeleteServiceProviderTypeAsync(int id)
        {
            var serviceProviderType = await unitOfWork.ServiceProviderTypes.GetByIdAsync(id);
            if (serviceProviderType == null)
            {
                throw new KeyNotFoundException("ServiceProviderType not found.");

            }

            unitOfWork.ServiceProviderTypes.Delete(serviceProviderType);
            await unitOfWork.SaveChangesAsync();

        }
        public async Task AddSpecializationAsync(SpecializationDTO specializationDTO)
        {
            if (specializationDTO == null)
            {
                throw new ArgumentException("specialization data is required.", nameof(specializationDTO));
            }

            // Validate ServiceProviderTypeId  (it must exist in the database)
            var Specialization = (unitOfWork.ServiceProviderTypes.GetAllQueryable())
                .FirstOrDefault(s => s.Id == specializationDTO.ServiceProviderTypeId);

            if (Specialization == null)
            {
                throw new ArgumentException("The provided ServiceProviderTypeId does not exist in the database.", nameof(specializationDTO.ServiceProviderTypeId));
            }

            // Check if the Specialization already exists in DB
            bool specializationExists = (unitOfWork.Specializations.GetAllQueryable()).Any(c => c.Name.Trim().ToLower() == specializationDTO.Name.Trim().ToLower());
            if (specializationExists)
            {
                throw new InvalidOperationException($"pecialization '{specializationDTO.Name}' already exists in the database.");
            }

            var specialization = new Specialization
            {
                Name = specializationDTO.Name,
                ServiceProviderTypeId = specializationDTO.ServiceProviderTypeId

            };
            await unitOfWork.Specializations.AddAsync(specialization);
            await unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteSpecializationAsync(int id)
        {
            var Specializations = await unitOfWork.Specializations.GetByIdAsync(id);
            if (Specializations == null)
            {
                throw new KeyNotFoundException("Specializations not found.");

            }

            unitOfWork.Specializations.Delete(Specializations);
            await unitOfWork.SaveChangesAsync();
        }




    }
}
