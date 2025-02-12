using Database.Models.Users;

namespace Database.Models
{
    public enum PaymentServiceType
    {
        Consultation,
        LegalService,
        Subscription,
        Other
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public class Payment
    {
        public int Id { get; set; }
        public string Signature { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceNumber { get; set; }
        public string PaymentGateway { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public PaymentServiceType ServiceType { get; set; }

        // Client relationship
        public int ClientId { get; set; }
        public Client Client { get; set; }

        // Service Provider relationship
        public int ServiceProviderId { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
    }
}