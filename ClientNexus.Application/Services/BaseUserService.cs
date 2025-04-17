using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Models;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services
{
    public class BaseUserService : IBaseUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaseUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // public async Task<IEnumerable<NotificationToken>> GetNotificationTokensAsync(
        //     IEnumerable<int> usersIds
        // )
        // {
        //     ArgumentNullException.ThrowIfNull(usersIds, nameof(usersIds));

        //     var tokens = await _unitOfWork.SqlGetListAsync<NotificationToken>(
        //         @$"
        //         select BaseUsers.NotificationToken as NotificationToken
        //         from ClientNexusSchema.BaseUsers
        //         where BaseUsers.Id in ({string.Join(",", usersIds)})
        //         "
        //     );

        //     if (tokens is null || !tokens.Any())
        //     {
        //         return Enumerable.Empty<NotificationToken>();
        //     }

        //     return tokens;
        // }

        // public async Task<NotificationToken?> GetNotificationTokenAsync(int userId)
        // {
        //     return await _unitOfWork.SqlGetSingleAsync<NotificationToken>(
        //         @$"
        //         select BaseUsers.NotificationToken as NotificationToken
        //         from ClientNexusSchema.BaseUsers
        //         where BaseUsers.Id = @userId
        //         ",
        //         new Parameter("userId", userId)
        //     );
        // }

        public async Task<bool> SetNotificationTokenAsync(int userId, string token)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(token);

            int rowsAffected = await _unitOfWork.SqlExecuteAsync(
                @"
                UPDATE ClientNexusSchema.BaseUsers SET NotificationToken = @token
                WHERE Id = @userId
            ",
                new Parameter("@token", token),
                new Parameter("@userId", userId)
            );

            return rowsAffected != 0;
        }
    }
}
