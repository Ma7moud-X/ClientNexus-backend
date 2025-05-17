namespace ClientNexus.Application.Interfaces
{
    public interface IBaseServiceService
    {
        Task<bool> AssignServiceProviderAsync(
            int serviceId,
            int clientId,
            int serviceProviderId,
            decimal price
        );
        Task CancelAsync(int serviceId);
        Task<bool> SetDoneAsync(int serviceId);
        Task<bool> UnassignServiceProviderAndSetPendingAsync(int serviceId);
    }
}
