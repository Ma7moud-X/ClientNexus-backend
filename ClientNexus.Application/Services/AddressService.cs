using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Entities.Others;


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
                throw new ArgumentNullException(nameof(addressDto), "Address data cannot be null.");

            var serviceProviderExists = await _unitOfWork.ServiceProviders.FirstOrDefaultAsync(sp => sp.Id == serviceProviderId);
            if (serviceProviderExists==null)
            {
                throw new ArgumentException("Service provider not found.");
            }
            

            
            var cityExists = await _unitOfWork.Cities.FirstOrDefaultAsync(c => c.Id == addressDto.CityId);
            if (cityExists==null)
            {
                throw new ArgumentException("City not found.");
            }

            var stateExists = await _unitOfWork.States.FirstOrDefaultAsync(s => s.Id == addressDto.StateId);
            if (stateExists==null)
            {
                throw new ArgumentException("State not found.");
            }
            if (cityExists.StateId != addressDto.StateId)
                throw new ArgumentException("The selected city does not belong to the specified state.");

            Address Address = new Address
            {
                BaseUserId = serviceProviderId,
                DetailedAddress = addressDto.DetailedAddress,
                CityId = addressDto.CityId,
                StateId= addressDto.StateId,
            };
            

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
