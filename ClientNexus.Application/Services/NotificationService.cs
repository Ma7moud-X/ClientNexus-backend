using System.Linq.Expressions;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
