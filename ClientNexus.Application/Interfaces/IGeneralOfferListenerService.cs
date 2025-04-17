using System;
using ClientNexus.Application.DTO;

namespace ClientNexus.Application.Interfaces;

public interface IGeneralOfferListenerService
{
    Task SubscribeAsync(int serviceId);
    Task<ClientOfferDTO> ListenAsync(CancellationToken cancellationToken);
    Task CloseAsync(bool save = false);
}
