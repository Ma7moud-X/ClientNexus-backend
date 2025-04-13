using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Services
{
    public class CountryService:IcountryService
    {
        private readonly IUnitOfWork unitOfWork;
        public CountryService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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
    }
}
