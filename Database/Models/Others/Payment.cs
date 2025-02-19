namespace Database.Models
{
    public enum PaymentStatus
    {
        Pending = 'P',
        Completed = 'C',
        Failed = 'F',
        Refunded = 'R',
    }

    public enum PaymentType
    {
        Subscription = 'S',
        Service = 'V',
    }

    public class Payment
    {
        public int Id { get; set; }
        public string Signature { get; set; } = default!;
        public decimal Amount { get; set; }
        public string ReferenceNumber { get; set; } = default!;
        public string PaymentGateway { get; set; } = default!;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}
