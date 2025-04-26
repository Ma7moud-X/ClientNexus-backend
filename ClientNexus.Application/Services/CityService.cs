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
    public class CityService: ICityServicecs
    {
        private readonly IUnitOfWork unitOfWork;
        public CityService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task AddCityAsync(CityDTO cityDTO)
        {
            if (cityDTO == null)
            {
                throw new ArgumentException("city data is required.", nameof(cityDTO));
            }
            var countries = unitOfWork.Countries.GetAllQueryable();
            var defaultCountry = await unitOfWork.Countries
                .FirstOrDefaultAsync(c => c.Name.Trim().ToLower() == "مصر".ToLower());

            if (defaultCountry == null)
            {
                throw new Exception("Default country (Egypt) not found in the database.");
            }

            // Validate StateId (it must exist in the database)
            var  city = (unitOfWork.States.GetAllQueryable())
                .FirstOrDefault(s => s.Id == cityDTO.StateId);

            if (city == null)
            {
                throw new ArgumentException("The provided StateId does not exist in the database.", nameof(cityDTO.StateId));
            }

            // Check if the city already exists in the same state
            var existingCities = (await unitOfWork.Cities.GetAllAsync())
             .Where(c => c.StateId == cityDTO.StateId)
               .ToList();

            bool CityExists = existingCities.Any(c =>
                NormalizeArabicName(c.Name) == NormalizeArabicName(cityDTO.Name)
            );
            if (CityExists)
            {
                throw new InvalidOperationException($"City '{cityDTO.Name}' already exists in the selected state.");
            }

            var City = new City
            {
                Name = cityDTO.Name,
                Abbreviation = cityDTO.Abbreviation,
                StateId = cityDTO.StateId,
                CountryId = defaultCountry.Id // Assign default country


            };
            await unitOfWork.Cities.AddAsync(City);
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
        private static string NormalizeArabicName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            name = name.Trim();

            // Remove "ال" if it starts with it
            if (name.StartsWith("ال"))
            {
                name = name.Substring(2);
            }

            return name.Trim().ToLower(); // Optional: make comparison case-insensitive
        }
    }
}
