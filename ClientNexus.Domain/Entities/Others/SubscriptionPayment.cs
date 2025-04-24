using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Entities.Others;

public class SubscriptionPayment : Payment
{
    public char SubscriptionType { get; set; } // M (Monthly), Q (Quarterly), Y (Yearly)
    public int ServiceProviderId { get; set; }
    public ServiceProvider ServiceProvider { get; set; }
    public string SubscriptionTier { get; set; } // Normal or Advanced
}
