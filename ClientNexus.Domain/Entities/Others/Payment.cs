using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Entities
{


 
        public class Payment
        {
            public int Id { get; set; }
            public string Signature { get; set; }
            public decimal Amount { get; set; }
            public string ReferenceNumber { get; set; }
            public string PaymentGateway { get; set; }
            public PaymentStatus Status { get; set; } // Uses enum
            public DateTime CreatedAt { get; set; }
            public PaymentType PaymentType { get; set; } // Uses enum
            public string IntentionId { get; set; }
            public string ClientSecret { get; set; }
            public string WebhookStatus { get; set; }
        }
    
}

