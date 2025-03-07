namespace ClientNexus.Domain.Interfaces;

public interface IPushNotification
{
    Task<string> SendNotificationAsync(string title, string body, string deviceToken, Dictionary<string, string>? data = null);
}
