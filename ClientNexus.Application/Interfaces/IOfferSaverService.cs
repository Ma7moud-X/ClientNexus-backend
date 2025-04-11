using ClientNexus.Application.DTO;

namespace ClientNexus.Application.Interfaces
{
    public interface IOfferSaverService
    {
        Task<bool> SaveAsync(ClientOfferDTO offerDTO, int serviceId, TimeSpan timeout);
    }
}
