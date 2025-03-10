namespace ClientNexus.Domain.Interfaces;

public interface IEventListener : IDisposable
{
    Task SubscribeAsync(string channel);
    Task<string> ListenAsync(CancellationToken cancellationToken);
}
