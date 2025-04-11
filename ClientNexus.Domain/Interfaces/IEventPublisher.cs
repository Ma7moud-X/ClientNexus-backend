namespace ClientNexus.Domain.Interfaces;

public interface IEventPublisher
{
    Task<long> PublishAsync(string channel, string message);
}
