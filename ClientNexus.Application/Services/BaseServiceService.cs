using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Enums;
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
            int affectedCount = 0;
            try
            {
                affectedCount = await _unitOfWork.SqlExecuteAsync(
                    @$"
                UPDATE ClientNexusSchema.Services SET Status = '{(char)ServiceStatus.InProgress}', Price = @price, ServiceProviderId = @serviceProviderId
                WHERE Status = '{(char)ServiceStatus.Pending}' AND ClientId = @clientId AND Id = @serviceId AND ServiceProviderId IS NULL;
                ",
                    new Parameter("@price", price),
                    new Parameter("@serviceProviderId", serviceProviderId),
                    new Parameter("@clientId", clientId),
                    new Parameter("@serviceId", serviceId)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Can't assign service provider to service", ex);
            }

            return affectedCount == 1;
        }

        public async Task CancelAsync(int serviceId)
        {
            int affectedCount;
            try
            {
                affectedCount = await _unitOfWork.SqlExecuteAsync(
                    @$"
                    UPDATE ClientNexusSchema.Services SET Status = '{(char)ServiceStatus.Cancelled}' WHERE Status IN ('{(char)ServiceStatus.Pending}', '{(char)ServiceStatus.Cancelled}') AND Id = @serviceId;
                    ",
                    new Parameter("@serviceId", serviceId)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Can't cancel service", ex);
            }

            if (affectedCount == 0)
            {
                throw new InvalidOperationException(
                    "Invalid operation. Service does not exist or can't be cancelled."
                );
            }
        }

        public async Task<bool> SetDoneAsync(int serviceId)
        {
            int affectedCount;
            try
            {
                affectedCount = await _unitOfWork.SqlExecuteAsync(
                    @$"
                    UPDATE ClientNexusSchema.Services SET Status = '{(char)ServiceStatus.Done}'
                    WHERE Status IN ('{(char)ServiceStatus.InProgress}', '{(char)ServiceStatus.Done}') AND Id = @serviceId;
                ",
                    new Parameter("@serviceId", serviceId)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Can't set service as done", ex);
            }

            return affectedCount == 1;
        }
    }
}
