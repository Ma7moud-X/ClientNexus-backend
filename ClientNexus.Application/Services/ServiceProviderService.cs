using ClientNexus.Application.Constants;
using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Models;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClientNexus.Application.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<BaseUser> _userManager;
        private readonly IFileService _fileService;

        public ServiceProviderService(
            IUnitOfWork unitOfWork,
            UserManager<BaseUser> userManager,
            IFileService fileService
        )
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _fileService = fileService;
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
                        sp.NotificationToken,
                        sp.PhoneNumber,
                        sp.IsDeleted,
                    }
                )
            ).FirstOrDefault();

            if (res is null)
            {
                throw new NotFoundException($"Service provider is not found");
            }

            var emergencyCase = (
                await _unitOfWork.EmergencyCases.GetByConditionAsync(
                    ec =>
                        ec.ServiceProviderId == serviceProviderId
                        && ec.Status == ServiceStatus.InProgress,
                    limit: 1
                )
            ).FirstOrDefault();

            return res.IsAvailableForEmergency
                && res.ApprovedById != null
                && res.BlockedById == null
                && res.NotificationToken != null
                && res.PhoneNumber != null
                && emergencyCase is null
                && !res.IsDeleted;
        }

        public async Task<bool> CheckIfAllowedToBeAvailableForEmergencyAsync(int serviceProviderId)
        {
            var res = (
                await _unitOfWork.ServiceProviders.GetByConditionAsync(
                    sp => sp.Id == serviceProviderId,
                    sp => new
                    {
                        sp.ApprovedById,
                        sp.BlockedById,
                        sp.NotificationToken,
                        sp.PhoneNumber,
                        sp.IsDeleted,
                    }
                )
            ).FirstOrDefault();

            if (res is null)
            {
                throw new NotFoundException($"Service provider is not found");
            }

            var emergencyCase = (
                await _unitOfWork.EmergencyCases.GetByConditionAsync(
                    ec =>
                        ec.CreatedAt >= DateTime.UtcNow.AddHours(-2)
                        && ec.ServiceProviderId == serviceProviderId
                        && ec.Status == ServiceStatus.InProgress,
                    limit: 1
                )
            ).FirstOrDefault();

            return res.ApprovedById != null
                && res.BlockedById == null
                && res.NotificationToken != null
                && res.PhoneNumber != null
                && emergencyCase is null
                && !res.IsDeleted;
        }

        public async Task<ServiceProviderOverview?> GetServiceProviderOverviewAsync(
            int serviceProviderId
        )
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

        public async Task<bool> SetAvailableForEmergencyAsync(int serviceProviderId)
        {
            var res = (
                await _unitOfWork.ServiceProviders.GetByConditionAsync(
                    sp => sp.Id == serviceProviderId,
                    sp => new
                    {
                        sp.ApprovedById,
                        sp.BlockedById,
                        sp.NotificationToken,
                    }
                )
            ).FirstOrDefault();

            if (res is null)
            {
                throw new NotFoundException($"Service provider is not found");
            }

            if (
                res.ApprovedById == null
                || res.BlockedById != null
                || res.NotificationToken == null
            )
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

        public async Task<bool> SetUnvavailableForEmergencyWithLockingAsync(int serviceProviderId)
        {
            var availability = await _unitOfWork.SqlGetSingleAsync<AvailableForEmergencyResponse>(
                @"
                SELECT IsAvailableForEmergency FROM ClientNexusSchema.ServiceProviders
                WITH (UPDLOCK, HOLDLOCK)
                WHERE Id = @serviceProviderId
                ",
                new Parameter("@serviceProviderId", serviceProviderId)
            );

            if (availability is null)
            {
                throw new NotFoundException("Service provider with is not found");
            }

            if (availability.IsAvailableForEmergency == false)
            {
                return false;
            }

            var affectedCount = await _unitOfWork.SqlExecuteAsync(
                @"
                UPDATE ClientNexusSchema.ServiceProviders SET IsAvailableForEmergency = 0
                WHERE Id = @serviceProviderId;
            ",
                new Parameter("@serviceProviderId", serviceProviderId)
            );

            return affectedCount == 1;
        }

        public async Task UpdateServiceProviderAsync(
            int ServiceProviderId,
            UpdateServiceProviderDTO updateDto
        )
        {
            if (updateDto == null)
            {
                throw new ArgumentNullException(nameof(updateDto), "Invalid request data.");
            }
            var serviceprovider =
                await _userManager.FindByIdAsync(ServiceProviderId.ToString()) as ServiceProvider;
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
            if (updateDto.MainImage != null)
            {
                var mainImageExtension = Path.GetExtension(updateDto.MainImage.FileName)
                    .TrimStart('.');
                var mainImageKey = $"{Guid.NewGuid()}.{mainImageExtension}";

                var mainImageType = _fileService.GetFileType(updateDto.MainImage);
                serviceprovider.MainImage = await _fileService.UploadPublicFileAsync(
                    updateDto.MainImage.OpenReadStream(),
                    mainImageType,
                    mainImageKey
                );
            }
            if (updateDto.Office_consultation_price != serviceprovider.Office_consultation_price)
                serviceprovider.Office_consultation_price = updateDto.Office_consultation_price;
            if (
                updateDto.Telephone_consultation_price
                != serviceprovider.Telephone_consultation_price
            )
                serviceprovider.Telephone_consultation_price =
                    updateDto.Telephone_consultation_price;
            if (updateDto.YearsOfExperience != serviceprovider.YearsOfExperience)
                serviceprovider.YearsOfExperience = updateDto.YearsOfExperience;
            if (updateDto.Description != serviceprovider.Description)
                serviceprovider.Description = updateDto.Description;
            if (updateDto.Email != serviceprovider.Email)
            {
                serviceprovider.Email = updateDto.Email;
                serviceprovider.UserName = updateDto.Email;
            }
            if (!string.IsNullOrWhiteSpace(updateDto.NewPassword))
            {
                // Check if the new password matches the current password
                var passwordMatches = await _userManager.CheckPasswordAsync(
                    serviceprovider,
                    updateDto.NewPassword
                );

                if (!passwordMatches)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(serviceprovider);
                    var passwordUpdateResult = await _userManager.ResetPasswordAsync(
                        serviceprovider,
                        token,
                        updateDto.NewPassword
                    );
                    if (!passwordUpdateResult.Succeeded)
                    {
                        string errors = string.Join(
                            ", ",
                            passwordUpdateResult.Errors.Select(e => e.Description)
                        );
                        throw new InvalidOperationException($"Password update failed: {errors}");
                    }
                }
            }
            var updateResult = await _userManager.UpdateAsync(serviceprovider);
            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException("ServiceProvider update failed.");
            }
        }

        private string NormalizeSearchQuery(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Trim().ToLower();

            // Remove Arabic "ال" prefix if present
            if (input.StartsWith("ال"))
            {
                input = input.Substring(2);
            }

            return input;
        }

        public async Task<List<ServiceProviderResponseDTO>> SearchServiceProvidersAsync(
            string? searchQuery
        )
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return new List<ServiceProviderResponseDTO>();

            searchQuery = NormalizeSearchQuery(searchQuery);

            var matchedSpecialization = await _unitOfWork.Specializations.FirstOrDefaultAsync(s =>
                s.Name.ToLower().Contains(searchQuery.ToLower())
            );

            var filteredServiceProviders = await _unitOfWork
                .ServiceProviders.GetAllQueryable()
                .AsNoTracking()
                .Where(sp=>sp.IsApproved==true)
                .Include(sp => sp.Addresses!)
                .ThenInclude(a => a.City!)
                .ThenInclude(c => c.State!)
                .Include(sp => sp.Specializations!)
                .Include(sp => sp.MainSpecialization!)
                .Where(sp =>
                    sp.FirstName.ToLower().Contains(searchQuery.ToLower())
                    || sp.LastName.ToLower().Contains(searchQuery.ToLower())
                    || (
                        matchedSpecialization != null
                        && sp.main_specializationID == matchedSpecialization.Id
                    )
                )
                .ToListAsync();

            var result = filteredServiceProviders
                .Select(sp => new ServiceProviderResponseDTO
                {
                    Id = sp.Id,
                    FirstName = sp.FirstName,
                    LastName = sp.LastName,
                    Rate = sp.Rate,
                    Description = sp.Description,
                    MainImage = sp.MainImage,
                    ImageIDUrl = sp.ImageIDUrl,
                    ImageNationalIDUrl = sp.ImageNationalIDUrl,
                    Gender = sp.Gender,
                    YearsOfExperience = sp.YearsOfExperience,
                    Office_consultation_price = sp.Office_consultation_price,
                    Telephone_consultation_price = sp.Telephone_consultation_price,
                    City = sp.Addresses?.FirstOrDefault()?.City?.Name,
                    State = sp.Addresses?.FirstOrDefault()?.City?.State?.Name,
                    main_Specialization = sp.MainSpecialization?.Name,
                    SpecializationName =
                        sp.Specializations?.Select(s => s.Name).ToList() ?? new List<string>(),
                })
                .ToList();

            return result;
        }

        public async Task<List<ServiceProviderResponseDTO>> FilterServiceProviderResponses(
            string searchQuery,
            float? minRate,
            string? state,
            string? city,
            string? specializationName
        )
        {
            var providers = await SearchServiceProvidersAsync(searchQuery);

            var filter = providers
                .Where(sp =>
                    // Rate filter (if provided)
                    (!minRate.HasValue || sp.Rate >= minRate.Value)
                    &&
                    // State filter (if provided)
                    (
                        string.IsNullOrEmpty(city)
                        || (
                            !string.IsNullOrEmpty(sp.City)
                            && NormalizeSearchQuery(sp.City)
                                .Equals(
                                    NormalizeSearchQuery(city),
                                    StringComparison.OrdinalIgnoreCase
                                )
                        )
                    )
                    &&
                    // City filter (if provided)
                    (
                        string.IsNullOrEmpty(city)
                        || (
                            !string.IsNullOrEmpty(sp.City)
                            && NormalizeSearchQuery(sp.City)
                                .Equals(
                                    NormalizeSearchQuery(city),
                                    StringComparison.OrdinalIgnoreCase
                                )
                        )
                    )
                    &&
                    // Specialization filter (if provided)
                    (
                        string.IsNullOrEmpty(specializationName)
                        || (
                            sp.main_Specialization != null
                            && sp.main_Specialization.Contains(
                                specializationName,
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                    )
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

            var serviceProviders = await _unitOfWork
                .ServiceProviders.GetAllQueryable()
                .AsNoTracking()
                .Include(sp => sp.Addresses!)
                .ThenInclude(a => a.City!)
                .ThenInclude(c => c.State!)
                .Include(sp => sp.Specializations!)
                .Where(sp => sp.IsApproved == IsApproved)
                .ToListAsync();

            var serviceProviderResponse = new List<ServiceProviderResponseDTO>();
            foreach (var sp in serviceProviders)
            {
                var mainSpec = await _unitOfWork.Specializations.FirstOrDefaultAsync(s =>
                    s.Id == sp.main_specializationID
                );

                var serviceProviderDto = new ServiceProviderResponseDTO
                {
                    Id = sp.Id,
                    FirstName = sp.FirstName,
                    LastName = sp.LastName,
                    Rate = sp.Rate,
                    Description = sp.Description,
                    MainImage = sp.MainImage,
                    ImageIDUrl = sp.ImageIDUrl,
                    ImageNationalIDUrl = sp.ImageNationalIDUrl,
                    Gender = sp.Gender,
                    YearsOfExperience = sp.YearsOfExperience,
                    Office_consultation_price = sp.Office_consultation_price,
                    Telephone_consultation_price = sp.Telephone_consultation_price,
                    City = sp.Addresses?.FirstOrDefault()?.City?.Name,
                    State = sp.Addresses?.FirstOrDefault()?.City?.State?.Name,
                    main_Specialization = mainSpec?.Name,
                    SpecializationName =
                        sp.Specializations?.Select(s => s.Name).ToList() ?? new List<string>(),
                };

                serviceProviderResponse.Add(serviceProviderDto);
            }

            return serviceProviderResponse;
        }

        public async Task<ServiceProviderResponseDTO> GetByIdAsync(int ServiceProviderId)
        {
            var sp = await _unitOfWork
                .ServiceProviders.GetAllQueryable()
                .AsNoTracking()
                .Include(s => s.Addresses!)
                .ThenInclude(a => a.City!)
                .ThenInclude(c => c.State!)
                .Include(s => s.Specializations!)
                .FirstOrDefaultAsync(s => s.Id == ServiceProviderId);

            if (sp == null)
            {
                throw new KeyNotFoundException("ServiceProviderId not found.");
            }

            var mainSpecialization = await _unitOfWork.Specializations.FirstOrDefaultAsync(s =>
                s.Id == sp.main_specializationID
            );

            return new ServiceProviderResponseDTO
            {
                Id = sp.Id,
                FirstName = sp.FirstName,
                LastName = sp.LastName,
                Rate = sp.Rate,
                Description = sp.Description,
                MainImage = sp.MainImage,
                ImageIDUrl = sp.ImageIDUrl,
                ImageNationalIDUrl = sp.ImageNationalIDUrl,
                YearsOfExperience = sp.YearsOfExperience,
                Gender = sp.Gender,
                Office_consultation_price = sp.Office_consultation_price,
                Telephone_consultation_price = sp.Telephone_consultation_price,
                City = sp.Addresses?.FirstOrDefault()?.City?.Name,
                State = sp.Addresses?.FirstOrDefault()?.City?.State?.Name,
                main_Specialization = mainSpecialization?.Name,
                SpecializationName =
                    sp.Specializations?.Select(s => s.Name).ToList() ?? new List<string>(),
            };
        }
    }
}
