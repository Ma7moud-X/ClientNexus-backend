using ClientNexus.Application.Models;

namespace ClientNexus.Application.Interfaces;

public interface IServiceProviderService
{
    Task<ServiceProviderOverview> GetServiceProviderOverviewAsync(int serviceProviderId);
}
