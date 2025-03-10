namespace ClientNexus.Domain.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync(string channel, string message);
}
