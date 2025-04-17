namespace ClientNexus.Application.Interfaces
{
    public interface IBaseUserService
    {
        // Task<IEnumerable<NotificationToken>> GetNotificationTokensAsync(IEnumerable<int> usersIds);
        Task<bool> SetNotificationTokenAsync(int userId, string token);
    }
}
