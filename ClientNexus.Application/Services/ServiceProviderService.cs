using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Services
{
    public class ServiceProviderService : IserviceProviderService
    {
        private readonly UserManager<BaseUser> _userManager;
        private readonly IUnitOfWork unitOfWork;
        public ServiceProviderService(UserManager<BaseUser> userManager, IUnitOfWork unitOfWork)
        {
            this._userManager = userManager;
            this.unitOfWork = unitOfWork;
        }
        public async Task UpdateServiceProviderAsync(int ServiceProviderId, UpdateServiceProviderDTO updateDto)
        {

            var serviceprovider = await _userManager.FindByIdAsync(ServiceProviderId.ToString()) as ServiceProvider;
            if (serviceprovider == null)
            {
                throw new KeyNotFoundException("ServiceProvider not found.");
            }
            if (updateDto.FirstName != serviceprovider.FirstName)

                serviceprovider.FirstName = updateDto.FirstName;
            if (updateDto.LastName != serviceprovider.LastName)
                serviceprovider.LastName = updateDto.LastName;
            if (updateDto.PhoneNumber != serviceprovider.PhoneNumber)
                serviceprovider.PhoneNumber = updateDto.PhoneNumber;
            if (updateDto.BirthDate != serviceprovider.BirthDate)
                serviceprovider.BirthDate = updateDto.BirthDate;
            if (updateDto.MainImage != serviceprovider.MainImage)
                serviceprovider.MainImage = updateDto.MainImage;
            if (updateDto.Email != serviceprovider.Email)
            {
                serviceprovider.Email = updateDto.Email;
                serviceprovider.UserName = updateDto.Email;
            }
            if (!string.IsNullOrWhiteSpace(updateDto.NewPassword))
            {

                // Check if the new password matches the current password
                var passwordMatches = await _userManager.CheckPasswordAsync(serviceprovider, updateDto.NewPassword);

                if (!passwordMatches)
                {


                    var token = await _userManager.GeneratePasswordResetTokenAsync(serviceprovider);
                    var passwordUpdateResult = await _userManager.ResetPasswordAsync(serviceprovider, token, updateDto.NewPassword);
                    if (!passwordUpdateResult.Succeeded)
                    {
                        string errors = string.Join(", ", passwordUpdateResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Password update failed: {errors}");
                    }
                }
            }
            var updateResult = await _userManager.UpdateAsync(serviceprovider);
            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException("Client update failed.");
            }



        }
        private string NormalizeSearchQuery(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            input = input.Trim().ToLower();

            // Remove Arabic "ال" prefix if present
            if (input.StartsWith("ال"))
            {
                input = input.Substring(2);
            }

            return input;
        }


        public async Task<List<ServiceProviderResponseDTO>> SearchServiceProvidersAsync(string? searchQuery)

        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return new List<ServiceProviderResponseDTO>();

            searchQuery = NormalizeSearchQuery(searchQuery);
            var serviceProviders = await unitOfWork.ServiceProviders.GetAllQueryable()
       .AsNoTracking()
       .Include(sp => sp.Addresses!)
           .ThenInclude(a => a.City!)
               .ThenInclude(c => c.State!)
       .Include(sp => sp.Specializations!) // Include Specializations
       .ToListAsync();



            var filteredProviders = serviceProviders
         .Where(sp =>
             sp.FirstName.ToLower().StartsWith(searchQuery.ToLower()) ||
             sp.LastName.ToLower().StartsWith(searchQuery.ToLower()) ||
            (sp.Specializations != null && sp.Specializations.Any(s => NormalizeSearchQuery(s.Name).Contains(searchQuery)))
         ).Select(sp => new ServiceProviderResponseDTO
         {
             FirstName = sp.FirstName,
             LastName = sp.LastName,
             Rate = sp.Rate,
             Description = sp.Description,
             MainImage = sp.MainImage,
             YearsOfExperience = sp.YearsOfExperience,
             City = sp.Addresses?.FirstOrDefault()?.City?.Name,
             State = sp.Addresses?.FirstOrDefault()?.City?.State?.Name,
             SpecializationName = sp.Specializations != null
                ? sp.Specializations.Select(s => s.Name).ToList()
                : new List<string>()

         })
        .ToList();

            return filteredProviders;
        }
        public async Task<List<ServiceProviderResponseDTO>> FilterServiceProviderResponses(string searchQuery, float? minRate, string? state, string? city, string? specializationName)
        {
            var providers = await SearchServiceProvidersAsync(searchQuery);


            var filter = providers
                .Where(sp =>
                    // Rate filter (if provided)
                    (!minRate.HasValue || sp.Rate >= minRate.Value) &&

                    // State filter (if provided)
                    (string.IsNullOrEmpty(state) ||
                        (!string.IsNullOrEmpty(sp.State) &&
                         sp.State.Equals(state, StringComparison.OrdinalIgnoreCase))) &&

                    // City filter (if provided)
                    (string.IsNullOrEmpty(city) ||
                (!string.IsNullOrEmpty(sp.City) &&
                sp.City.Equals(city, StringComparison.OrdinalIgnoreCase)))


                 &&

                    // Specialization filter (if provided)
                    (string.IsNullOrEmpty(specializationName) ||
                        (sp.SpecializationName != null &&
                         sp.SpecializationName.Any(s =>
                             s.Contains(specializationName, StringComparison.OrdinalIgnoreCase))))
                 )
                 .ToList();

            return filter;
        }
        public async Task<List<ServiceProviderResponseDTO>> GetAllServiceProvider(bool? IsApproved)
        {
            if (IsApproved == null)
            {
                return new List<ServiceProviderResponseDTO>();
            }

            var serviceProviders = await unitOfWork.ServiceProviders.GetAllQueryable()
                   .AsNoTracking()
                   .Include(sp => sp.Addresses!)
                       .ThenInclude(a => a.City!)
                           .ThenInclude(c => c.State!)
                   .Include(sp => sp.Specializations!) // Include Specializations
                   .ToListAsync();

            var serviceProviderResponse = serviceProviders
       .Where(sp =>
           sp.IsApproved == IsApproved 
           
       ).Select(sp => new ServiceProviderResponseDTO
       {
           FirstName = sp.FirstName,
           LastName = sp.LastName,
           Rate = sp.Rate,
           ImageIDUrl= sp.ImageIDUrl,
           ImageNationalIDUrl= sp.ImageNationalIDUrl,
           Description = sp.Description,
           MainImage = sp.MainImage,
           YearsOfExperience = sp.YearsOfExperience,
           City = sp.Addresses?.FirstOrDefault()?.City?.Name,
           State = sp.Addresses?.FirstOrDefault()?.City?.State?.Name,
           SpecializationName = sp.Specializations != null
              ? sp.Specializations.Select(s => s.Name).ToList()
              : new List<string>()

       }).ToList();


            return serviceProviderResponse;
        }



    }
}
