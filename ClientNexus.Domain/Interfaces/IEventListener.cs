namespace ClientNexus.Domain.Interfaces;

public interface IEventListener
{
    Task SubscribeAsync(string channel);
    Task<string> ListenAsync(CancellationToken cancellationToken);
    Task CloseAsync(bool save = false, string? saveToListAtKey = null);
}
