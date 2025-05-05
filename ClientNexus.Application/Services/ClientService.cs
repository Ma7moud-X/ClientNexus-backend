using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Services
{
    public class ClientService:IClientService

    {
        private readonly UserManager<BaseUser> _userManager;
        private readonly IFileService _fileService;

        public ClientService(UserManager<BaseUser> userManager, IFileService fileService)
        {
            this._userManager = userManager;
            _fileService = fileService;
        }
        public async Task UpdateClientAsync(int ClientId, UpdateClientDTO updateDto)
        {
            if (updateDto == null)
            {
                throw new ArgumentNullException(nameof(updateDto), "Invalid request data.");
            }

            var client = await _userManager.FindByIdAsync(ClientId.ToString()) as Client;
            if (client == null)
            {
                throw new KeyNotFoundException("Client not found.");
            }
            if (updateDto.FirstName != client.FirstName)

                client.FirstName = updateDto.FirstName;
            if (updateDto.LastName != client.LastName)
                client.LastName = updateDto.LastName;
            if (updateDto.PhoneNumber != client.PhoneNumber)
                client.PhoneNumber = updateDto.PhoneNumber;
            if (updateDto.BirthDate !=client.BirthDate)
                client.BirthDate = updateDto.BirthDate;
            if (updateDto.MainImage != null)
            {
                var mainImageExtension = Path.GetExtension(updateDto.MainImage.FileName).TrimStart('.');
                var mainImageKey = $"{Guid.NewGuid()}.{mainImageExtension}";
                var mainImageType = _fileService.GetFileType(updateDto.MainImage);
                client.MainImage = await _fileService.UploadPublicFileAsync(updateDto.MainImage.OpenReadStream(), mainImageType, mainImageKey);

            }
            if (updateDto.Email != client.Email)
            {
                client.Email = updateDto.Email;
                client.UserName = updateDto.Email;
            }
            if (!string.IsNullOrWhiteSpace(updateDto.NewPassword))
            {

                // Check if the new password matches the current password
                var passwordMatches = await _userManager.CheckPasswordAsync(client, updateDto.NewPassword);
                if (!passwordMatches)
                {


                    var token = await _userManager.GeneratePasswordResetTokenAsync(client);
                    var passwordUpdateResult = await _userManager.ResetPasswordAsync(client, token, updateDto.NewPassword);
                    if (!passwordUpdateResult.Succeeded)
                    {
                        string errors = string.Join(", ", passwordUpdateResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Password update failed: {errors}");
                    }
                }
            }
            var updateResult = await _userManager.UpdateAsync(client);
            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException("Client update failed.");
            }



        }

      

        public async Task<ClientResponseDTO> GetByIdAsync(int clientId)
        {

            var client = await _userManager.FindByIdAsync(clientId.ToString()) as Client;
            if (client == null)
            {
                throw new KeyNotFoundException("Client not found.");
            }

            return new ClientResponseDTO
            {
                Email= client.Email,
                PlainPassword=client.PlainPassword,
                FirstName = client.FirstName,
                LastName = client.LastName,
                PhoneNumber = client.PhoneNumber,
                BirthDate = client.BirthDate,
                Gender = client.Gender,
                MainImage = client.MainImage

            };
        }
    }
}
