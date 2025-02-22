using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Entities.Others;

public class SubscriptionPayment : Payment
{
    public SubscriptionType SubscriptionType { get; set; }

    public int ServiceProviderId { get; set; }
    public ServiceProvider? ServiceProvider { get; set; }
}
