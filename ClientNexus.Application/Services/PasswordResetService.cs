using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
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


    public class PasswordResetService : IPasswordResetService
    {
        private readonly IOtpService _otpService;
        private readonly UserManager<BaseUser> _userManager;
        private readonly IClientService _clientService;
        private readonly IServiceProviderService _serviceProviderService;

        public PasswordResetService(
            IOtpService otpService,
            UserManager<BaseUser> userManager,
            IClientService clientService,
            IServiceProviderService serviceProviderService)
        {
            _otpService = otpService;
            _userManager = userManager;
            _clientService = clientService;
            _serviceProviderService = serviceProviderService;
        }

        public async Task ResetPasswordAsync(PasswordResetRequestDTO request)
        {
            // Validate OTP
            var isOtpValid = await _otpService.ValidateOtpAsync(request.Email, request.Otp);
            if (!isOtpValid)
            {
                throw new InvalidOperationException("Invalid OTP.");
            }

            // Find user by email using UserManager
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Check if user is a Client or ServiceProvider
            if (user is Client client)
            {
                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(client);
                var result = await _userManager.ResetPasswordAsync(client, token, request.NewPassword);

                if (!result.Succeeded)
                {
                    string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Password reset failed: {errors}");
                }
            }
            else if (user is ServiceProvider serviceProvider)
            {
                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(serviceProvider);
                var result = await _userManager.ResetPasswordAsync(serviceProvider, token, request.NewPassword);

                if (!result.Succeeded)
                {
                    string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Password reset failed: {errors}");
                }
            }
            else
            {
                throw new InvalidOperationException("User is neither Client nor ServiceProvider.");
            }
        }
    }


}
