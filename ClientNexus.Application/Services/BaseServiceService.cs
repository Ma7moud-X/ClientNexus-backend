using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services
{
    public class BaseServiceService : IBaseServiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaseServiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AssignServiceProviderAsync(
            int serviceId,
            int clientId,
            int serviceProviderId,
            decimal price
        )
        {
            int affectedCount = await _unitOfWork.SqlExecuteAsync(
                @$"
                UPDATE ClientNexusSchema.Services SET Status = '{(char)ServiceStatus.InProgress}', Price = @price, ServiceProviderId = @serviceProviderId
                WHERE Status = '{(char)ServiceStatus.Pending}' AND ClientId = @clientId AND Id = @serviceId AND ServiceProviderId IS NULL;
                ",
                new Parameter("@price", price),
                new Parameter("@serviceProviderId", serviceProviderId),
                new Parameter("@clientId", clientId),
                new Parameter("@serviceId", serviceId)
            );

            return affectedCount == 1;
        }

        public async Task CancelAsync(int serviceId)
        {
            int affectedCount = await _unitOfWork.SqlExecuteAsync(
                @$"
                    UPDATE ClientNexusSchema.Services SET Status = '{(char)ServiceStatus.Cancelled}' WHERE Status IN ('{(char)ServiceStatus.Pending}', '{(char)ServiceStatus.Cancelled}') AND Id = @serviceId;
                    ",
                new Parameter("@serviceId", serviceId)
            );

            if (affectedCount == 0)
            {
                throw new NotAllowedException(
                    "Invalid operation. Service does not exist or can't be cancelled."
                );
            }
        }

        public async Task<bool> SetDoneAsync(int serviceId)
        {
            int affectedCount = await _unitOfWork.SqlExecuteAsync(
                @$"
                    UPDATE ClientNexusSchema.Services SET Status = '{(char)ServiceStatus.Done}'
                    WHERE Status IN ('{(char)ServiceStatus.InProgress}', '{(char)ServiceStatus.Done}') AND Id = @serviceId;
                ",
                new Parameter("@serviceId", serviceId)
            );

            return affectedCount == 1;
        }
    }
}
