using ClientNexus.Domain.Entities.Users;

namespace ClientNexus.Domain.Entities.Others;

public class SubscriptionPayment : Payment
{
    public int ServiceProviderId { get; set; }
    public ServiceProvider? ServiceProvider { get; set; }
}
