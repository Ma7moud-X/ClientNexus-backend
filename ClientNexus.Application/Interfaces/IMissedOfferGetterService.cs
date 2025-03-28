using ClientNexus.Application.DTO;

namespace ClientNexus.Application.Interfaces;

public interface IMissedOfferGetterService
{
    Task<IEnumerable<ClientOfferDTO?>?> GetAllAsync(int serviceId);
}
