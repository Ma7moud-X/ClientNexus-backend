using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientNexus.Domain.Entities;


namespace ClientNexus.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task AddAddressAsync(int serviceProviderId, AddressDTO addressDto)
        {
            if (addressDto == null)
            {
                throw new ArgumentNullException(nameof(addressDto), "Address data cannot be null.");
            }
            Address Address = new Address
            {
                BaseUserId = serviceProviderId,
                DetailedAddress = addressDto.DetailedAddress,
                Neighborhood = addressDto.Neighborhood,
                MapUrl = addressDto.MapUrl,
                CityId = addressDto.CityId
            };
            if (_unitOfWork.Addresses == null)
            {
                throw new InvalidOperationException("Address repository is not initialized.");
            }

            await _unitOfWork.Addresses.AddAsync(Address);
            await _unitOfWork.SaveChangesAsync();

        }

        //public async Task<IEnumerable<Address>> GetAddressesByServiceProviderAsync(int serviceProviderId)
        //{
        //    return (await unitOfWork.Addresses.GetAllAsync())
        //        .Where(a => a.BaseUserId == serviceProviderId)
        //        .ToList();
        //}

        public async Task DeleteAddressAsync(int addressId)
        {
            var address = _unitOfWork.Addresses.GetAllQueryable().FirstOrDefault(a => a.Id == addressId);
            if (address == null)
            {
                throw new KeyNotFoundException("Address not found.");

            }

            _unitOfWork.Addresses.Delete(address);
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
