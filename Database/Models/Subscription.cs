
namespace Database.Models
{
    public enum SubscriptionStatus
    {
        Active,
        Expired,
        Suspended,
        Cancelled
    }
    public enum SubscriptionType
    {
        Free,
        Basic,
        Premium
    }
    public class Subscription
    {
        public int Id { get; set; }
        public SubscriptionType Type { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}