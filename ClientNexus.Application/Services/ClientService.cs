﻿using ClientNexus.Application.DTOs;
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
    public class ClientService:IClientService

    {
        private readonly UserManager<BaseUser> _userManager;
 
        public ClientService(UserManager<BaseUser> userManager)
        {
            this._userManager = userManager;
           
        }
        public async Task UpdateClientAsync(int ClientId, UpdateClientDTO updateDto)
        {

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
    }
}
