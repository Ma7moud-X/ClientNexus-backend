using Database.Models.Users;

namespace Database.Models.Others;

public class SubscriptionPayment : Payment
{
    public int ServiceProviderId { get; set; }
    public ServiceProvider? ServiceProvider { get; set; }
}
