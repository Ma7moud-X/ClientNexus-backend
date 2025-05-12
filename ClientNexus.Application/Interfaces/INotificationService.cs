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
    }
}
