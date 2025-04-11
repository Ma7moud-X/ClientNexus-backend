using ClientNexus.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IAddressService
    {
        public Task AddAddressAsync(int serviceProviderId, AddressDTO addressDto);
        //public Task<IEnumerable<Address>> GetAddressesByServiceProviderAsync(int serviceProviderId);
        public Task DeleteAddressAsync(int addressId);
    }
}
