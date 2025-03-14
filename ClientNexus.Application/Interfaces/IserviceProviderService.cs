using ClientNexus.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IserviceProviderService
    {
        public Task<List<ServiceProviderResponse>> SearchServiceProvidersAsync(string? searchQuery);
        public Task UpdateServiceProviderAsync(int ServiceProviderId, UpdateServiceProviderDTO updateDto);
        public Task<List<ServiceProviderResponse>> FilterServiceProviderResponses(string searchQuery, float? minRate, string? state, string? city, string? specializationName);



    }
}
