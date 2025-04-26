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
    public class serviceProviderTypeService: IServiceProviderTypeService
    {
        private readonly IUnitOfWork unitOfWork;
        public serviceProviderTypeService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task AddServiceProviderTypeAsyn(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException(" Name is required.", nameof(Name));
            }
            // Check if a ServiceProviderType with the same name already exists
            var serviceProviderTypes = await unitOfWork.ServiceProviderTypes.GetAllQueryable().ToListAsync();


            bool exists = serviceProviderTypes.Any(s =>
                NormalizeArabicName(s.Name) == NormalizeArabicName(Name)
            );
            if (exists)
            {
                throw new InvalidOperationException($"ServiceProviderType '{Name}' already exists.");

            }
            ServiceProviderType serviceProviderType = new ServiceProviderType()
            {
                Name = Name
            };
            await unitOfWork.ServiceProviderTypes.AddAsync(serviceProviderType);
            await unitOfWork.SaveChangesAsync();


        }
        public async Task DeleteServiceProviderTypeAsync(int id)
        {
            var serviceProviderType = await unitOfWork.ServiceProviderTypes.GetByIdAsync(id);
            if (serviceProviderType == null)
            {
                throw new KeyNotFoundException("ServiceProviderType not found.");

            }

            unitOfWork.ServiceProviderTypes.Delete(serviceProviderType);
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
