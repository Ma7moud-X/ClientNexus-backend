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
    public class StateService: IStateService
    {
        private readonly IUnitOfWork unitOfWork;
        public StateService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task AddStateAsync(StateDTO stateDTO)
        {
            if (stateDTO == null)
            {
                throw new ArgumentException("state name is required.", nameof(stateDTO));
            }
            var countries = unitOfWork.Countries.GetAllQueryable();
            var defaultCountry = await unitOfWork.Countries
                .FirstOrDefaultAsync(c => c.Name.Trim().ToLower() == "مصر".ToLower());

            if (defaultCountry == null)
            {
                throw new Exception("Default country (Egypt) not found in the database.");
            }
            // Check if the state already exists
            var existingStates = (await unitOfWork.States.GetAllAsync())
              .Where(s => s.CountryId == defaultCountry.Id)
                .ToList();

            bool stateExists = existingStates.Any(s =>
                NormalizeArabicName(s.Name) == NormalizeArabicName(stateDTO.Name)
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
        public async Task<List<StateResponseDTO>> GetAllStatesAsync()
        {
            var states = await unitOfWork.States.GetAllAsync();

            var stateDTOs = states.Select(state => new StateResponseDTO
            {
                Id = state.Id,
                Name = state.Name,
                Abbreviation = state.Abbreviation,
                CountryId = state.CountryId
            }).ToList();

            return stateDTOs;
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
