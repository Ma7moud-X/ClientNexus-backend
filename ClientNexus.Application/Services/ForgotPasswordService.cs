//using ClientNexus.Application.DTOs;
//using ClientNexus.Domain.Entities.Users;
//using FirebaseAdmin.Auth;
//using Microsoft.AspNetCore.Identity;

//using System;
//using System.Threading.Tasks;

//namespace ClientNexus.Application.Services
//{
//    public class ForgotPasswordService
//    {
//        private readonly UserManager<BaseUser> _userManager;

//        public ForgotPasswordService(UserManager<BaseUser> userManager)
//        {
//            _userManager = userManager;
//        }

//        // Step A: Send the OTP (reset link)
//        public async Task SendOtpAsync(string email)
//        {
//            try
//            {
//                FirebaseService.InitializeFirebase();
//                string resetLink = await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(email);
//                // You must actually email this link to the user (or log it).
//                Console.WriteLine("Reset link generated for " + email);
//                Console.WriteLine("Link: " + resetLink);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error sending OTP: " + ex.Message);
//                throw new Exception("Unable to send OTP email.");
//            }
//        }

//        // Step B: Confirm the oobCode, then update the user's password in .NET Identity
//        public async Task ConfirmResetAsync(
//            ConfirmResetPasswordDTO dto,
//            ClientService clientService,
//            ServiceProviderService serviceProviderService)
//        {
//            // 1) Verify the oobCode with Firebase
//            FirebaseService.InitializeFirebase();
//            string verifiedEmail = await FirebaseAuth.DefaultInstance.VerifyPasswordResetCodeAsync(dto.OobCode);
//            verifiedEmail = await FirebaseAuth.DefaultInstance.VerifyPasswordResetCodeAsync(dto.OobCode);
//            await FirebaseAuth.DefaultInstance.ConfirmPasswordResetAsync(dto.OobCode, dto.NewPassword);
//            // 2) Retrieve the existing user by ID
//            var baseUser = await _userManager.FindByIdAsync(dto.UserId.ToString());
//            if (baseUser == null)
//            {
//                throw new Exception("User not found in local database.");
//            }

//            // 3) Double-check that the user’s existing email matches or that we accept this new verified email
//            //    Because we're about to set baseUser.Email = verifiedEmail in the update method.
//            //    If your business rules require that we confirm they match, do so here.
//            //    If not, just proceed to update the email with the verifiedEmail.

//            // 4) Depending on whether it's a Client or a ServiceProvider, call the relevant update method.
//            if (dto.IsClient)
//            {
//                // Cast to Client
//                var client = baseUser as Client;
//                if (client == null)
//                {
//                    throw new Exception("User is not a Client.");
//                }

//                // Prepare an UpdateClientDTO with the user's *existing* data so we don't overwrite it
//                var updateDto = new UpdateClientDTO
//                {
//                    // We keep their existing values:
//                    FirstName = client.FirstName,
//                    LastName = client.LastName,
//                    BirthDate = client.BirthDate,
//                    PhoneNumber = client.PhoneNumber,

//                    // We update only Email (verified from Firebase) and the NewPassword
//                    Email = verifiedEmail,
//                    NewPassword = dto.NewPassword
//                };

//                await clientService.UpdateClientAsync(dto.UserId, updateDto);
//            }
//            else
//            {
//                // Cast to ServiceProvider
//                var serviceProvider = baseUser as ServiceProvider;
//                if (serviceProvider == null)
//                {
//                    throw new Exception("User is not a ServiceProvider.");
//                }

//                // Prepare an UpdateServiceProviderDTO with the user's existing data
//                var updateDto = new UpdateServiceProviderDTO
//                {
//                    FirstName = serviceProvider.FirstName,
//                    LastName = serviceProvider.LastName,
//                    BirthDate = serviceProvider.BirthDate,
//                    PhoneNumber = serviceProvider.PhoneNumber,
//                    MainImage = serviceProvider.MainImage,

//                    // Overwrite only Email and NewPassword
//                    Email = verifiedEmail,
//                    NewPassword = dto.NewPassword
//                };

//                await serviceProviderService.UpdateServiceProviderAsync(dto.UserId, updateDto);
//            }
//        }
    
    
    
    
//    }
//}
