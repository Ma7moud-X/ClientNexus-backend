using ClientNexus.Application.DTO;

namespace ClientNexus.Application.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDTO> CreateNotificationAsync(int userId, string title, string body);
        Task<NotificationDTO?> GetNotificationByIdAsync(Ulid id);
        Task<IEnumerable<NotificationDTO>> GetLatestUserNotificationsAsync(
            int userId,
            int limit,
            Ulid? belowUlid = null
        );
        Task<bool> SendNotificationAsync(
            string title,
            string body,
            int userId,
            Dictionary<string, string>? data = null
        );
        Task<bool> SendNotificationAsync(
            string title,
            string body,
            int userId,
            string notificationToken,
            Dictionary<string, string>? data = null
        );
        Task SendNotificationToServiceProvidersNearLocationAsync(
            double longitude,
            double latitude,
            double radiusInMeters,
            string title,
            string body,
            Dictionary<string, string>? data = null
        );
        Task<bool> RemoveNotificationTokenAsync(int userId);
    }
}
