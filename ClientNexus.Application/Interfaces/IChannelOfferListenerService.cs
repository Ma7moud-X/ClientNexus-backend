using ClientNexus.Application.DTO;

namespace ClientNexus.Application.Interfaces;

public interface IChannelOfferListenerService
{
    Task SubscribeAsync(int serviceId);
    Task<ClientOfferDTO> ListenAsync(CancellationToken cancellationToken);
    Task CloseAsync(bool save = false);
}
