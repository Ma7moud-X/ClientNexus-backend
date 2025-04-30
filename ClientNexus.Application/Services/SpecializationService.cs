using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
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
            var serviceProviderExists = await unitOfWork.ServiceProviders.FirstOrDefaultAsync(sp => sp.Id == serviceProviderId);
            if (serviceProviderExists == null)
            {
                throw new ArgumentException("Service provider not found.");
            }

            if (SpecializationIDs == null || !SpecializationIDs.Any())
            {
                return;
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


        public async Task AddNewSpecializationAsync(SpecializationDTO specializationDTO)
        {
            if (specializationDTO == null)
            {
                throw new ArgumentException("specialization data is required.", nameof(specializationDTO));
            }

            // Validate ServiceProviderTypeId  (it must exist in the database)
            var ServiceProviderTypeIdExits = (unitOfWork.ServiceProviderTypes.GetAllQueryable())
                .FirstOrDefault(s => s.Id == specializationDTO.ServiceProviderTypeId);

            if (ServiceProviderTypeIdExits == null)
            {
                throw new ArgumentException("The provided ServiceProviderTypeId does not exist in the database.", nameof(specializationDTO.ServiceProviderTypeId));
            }

            // Check if the Specialization already exists in DB
            var specializations = await unitOfWork.Specializations.GetAllQueryable().ToListAsync();


            bool specializationExists = specializations.Any(s =>
                NormalizeArabicName(s.Name) == NormalizeArabicName(specializationDTO.Name)
            );
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
        public async Task<List<SpecializationResponseDTO>> GetAllSpecializationsAsync()
        {
            var specializations = await unitOfWork.Specializations.GetAllQueryable().ToListAsync();

         
            var SpecializationResponseDTOs = specializations.Select(s => new SpecializationResponseDTO
            {
                Id = s.Id,
                Name = s.Name,
                ServiceProviderTypeId = s.ServiceProviderTypeId
            }).ToList();

            return SpecializationResponseDTOs;
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
