using System.Linq.Expressions;
using ClientNexus.Application.Constants;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Domain.ValueObjects;

namespace ClientNexus.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPushNotification _pushNotification;

        public NotificationService(IUnitOfWork unitOfWork, IPushNotification pushNotification)
        {
            _unitOfWork = unitOfWork;
            _pushNotification = pushNotification;
        }

        public async Task<NotificationDTO> CreateNotificationAsync(
            int userId,
            string title,
            string body
        )
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new InvalidOperationException($"{nameof(title)} can't be null");
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new InvalidOperationException($"{nameof(body)} can't be null");
            }

            Notification createdNotif = await _unitOfWork.Notifications.AddAsync(
                new Notification
                {
                    Title = title,
                    Body = body,
                    BaseUserId = userId,
                }
            );
            int createdCount = await _unitOfWork.SaveChangesAsync();
            if (createdCount == 0)
            {
                throw new ServerException(
                    "Unexpected error occured. Notification should have been created"
                );
            }

            return new NotificationDTO
            {
                Id = createdNotif.Id,
                Title = createdNotif.Title,
                Body = createdNotif.Body,
                CreatedAt = createdNotif.CreatedAt,
                UserId = createdNotif.BaseUserId,
            };
        }

        public async Task<IEnumerable<NotificationDTO>> GetLatestUserNotificationsAsync(
            int userId,
            int limit,
            Ulid? belowUlid = null
        )
        {
            Expression<Func<Notification, bool>> condExp;
            if (belowUlid is null)
            {
                condExp = n => n.BaseUserId == userId;
            }
            else
            {
                condExp = n => n.BaseUserId == userId && n.Id.CompareTo(belowUlid.Value) < 0;
            }

            return await _unitOfWork.Notifications.GetByConditionAsync(
                condExp: condExp,
                selectExp: n => new NotificationDTO
                {
                    Id = n.Id,
                    Title = n.Title,
                    Body = n.Body,
                    CreatedAt = n.CreatedAt,
                    UserId = n.BaseUserId,
                },
                limit: limit,
                orderByExp: n => n.Id,
                descendingOrdering: true
            );
        }

        public async Task<NotificationDTO?> GetNotificationByIdAsync(Ulid id)
        {
            NotificationDTO? not = (
                await _unitOfWork.Notifications.GetByConditionAsync(
                    condExp: n => n.Id == id,
                    limit: 1,
                    selectExp: n => new NotificationDTO
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Body = n.Body,
                        CreatedAt = n.CreatedAt,
                        UserId = n.BaseUserId,
                    },
                    orderByExp: n => n.Id,
                    descendingOrdering: true
                )
            ).FirstOrDefault();

            return not;
        }

        public async Task<bool> RemoveNotificationTokenAsync(int userId)
        {
            int rowsAffected = await _unitOfWork.SqlExecuteAsync(
                @"
                UPDATE ClientNexusSchema.BaseUsers SET NotificationToken = NULL
                WHERE Id = @userId
            ",
                new Parameter("@userId", userId)
            );

            return rowsAffected != 0;
        }

        public async Task<bool> SendNotificationAsync(
            string title,
            string body,
            int userId,
            Dictionary<string, string>? data = null
        )
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new InvalidOperationException($"{nameof(title)} can't be null");
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new InvalidOperationException($"{nameof(body)} can't be null");
            }

            var notificationToken = (
                await _unitOfWork.BaseUsers.GetByConditionAsync(
                    condExp: u => u.Id == userId,
                    selectExp: u => u.NotificationToken
                )
            ).FirstOrDefault();

            if (notificationToken is null)
            {
                throw new NotFoundException("Notification token not found");
            }

            return await SendNotificationAsync(title, body, userId, notificationToken, data);
        }

        public async Task<bool> SendNotificationAsync(
            string title,
            string body,
            int userId,
            string notificationToken,
            Dictionary<string, string>? data = null
        )
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new InvalidOperationException($"{nameof(title)} can't be null");
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new InvalidOperationException($"{nameof(body)} can't be null");
            }

            if (string.IsNullOrWhiteSpace(notificationToken))
            {
                throw new InvalidOperationException($"{nameof(notificationToken)} can't be null");
            }

            bool isSent = false;
            try
            {
                await _pushNotification.SendNotificationAsync(title, body, notificationToken, data);
                isSent = true;
            }
            catch (DeviceNotRegisteredException)
            {
                await RemoveNotificationTokenAsync(userId);
            }
            catch (MessageRateExceeded)
            {
                int retryCount = 0;
                while (retryCount < 3)
                {
                    retryCount++;
                    await Task.Delay(1000 * retryCount);
                    try
                    {
                        await _pushNotification.SendNotificationAsync(
                            title,
                            body,
                            notificationToken,
                            data
                        );
                        isSent = true;
                    }
                    catch (DeviceNotRegisteredException)
                    {
                        await RemoveNotificationTokenAsync(userId);
                        break;
                    }
                    catch (MessageRateExceeded) { }
                }
            }

            if (isSent)
            {
                await CreateNotificationAsync(userId, title, body);
            }

            return isSent;
        }

        public async Task SendNotificationToServiceProvidersNearLocationAsync(
            double longitude,
            double latitude,
            double radiusInMeters,
            string title,
            string body,
            Dictionary<string, string>? data = null
        )
        {
            var serviceProvidersDetails = await _unitOfWork.ServiceProviders.GetByConditionAsync(
                sp =>
                    sp.CurrentLocation != null
                    && sp.CurrentLocation.Distance(new MapPoint(longitude, latitude))
                        <= radiusInMeters
                    && sp.LastLocationUpdateTime != null
                    && sp.LastLocationUpdateTime
                        > DateTime.UtcNow.AddMinutes(
                            -GlobalConstants.ServiceProviderLocationValiditySpanInMinutes
                        )
                    && sp.NotificationToken != null,
                sp => new { Token = sp.NotificationToken!, sp.Id }
            );

            foreach (var detail in serviceProvidersDetails)
            {
                await SendNotificationAsync(title, body, detail.Id, detail.Token, data);
            }
        }
    }
}
