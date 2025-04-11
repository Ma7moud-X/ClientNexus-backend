namespace ClientNexus.Domain.Interfaces;

public interface IEventSubscriber : IDisposable
{
    Task SubscribeAsync(string channel, Action<string> messageHandler);
    void Unsubscribe();
}
