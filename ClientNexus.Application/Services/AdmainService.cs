using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClientNexus.Application.Services
{
    public class AdmainService : IAdmainService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<BaseUser> _userManager;

        public AdmainService(IUnitOfWork unitOfWork, UserManager<BaseUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this._userManager = userManager;
        }

        public async Task ApprovingServiceProviderAsync(int ServiceID, int adminId)
        {
            // Retrieve the service provider from the database
            var serviceProvider = await unitOfWork.ServiceProviders.GetByIdAsync(ServiceID);

            // Check if service provider exists
            if (serviceProvider == null)
            {
                throw new KeyNotFoundException($"Service provider with ID {ServiceID} not found.");
            }

            serviceProvider.IsApproved = true;
            serviceProvider.ApprovedById = adminId;
            await unitOfWork.SaveChangesAsync();
        }
    }
}
