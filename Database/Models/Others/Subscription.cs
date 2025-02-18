using Database.Models.Users;

namespace Database.Models
{
    public enum SubscriptionStatus
    {
        Active = 'A',
        Expired = 'E',
        Suspended = 'S',
        Cancelled = 'C',
    }

    public enum SubscriptionType
    {
        Free = 'F',
        Basic = 'B',
        Premium = 'P',
    }

    public class Subscription
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public SubscriptionType Type { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime ExpireDate { get; set; }

        public int ServiceProviderId { get; set; }
        public ServiceProvider? ServiceProvider { get; set; }
    }
}
