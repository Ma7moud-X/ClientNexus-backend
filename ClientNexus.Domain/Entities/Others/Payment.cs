using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Entities
{

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
