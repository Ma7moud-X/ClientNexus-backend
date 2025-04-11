using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientNexus.Application.Models;

namespace ClientNexus.Application.Interfaces
{
    public interface IBaseUserService
    {
        Task<IEnumerable<NotificationToken>> GetNotificationTokensAsync(IEnumerable<int> usersIds);
    }
}
