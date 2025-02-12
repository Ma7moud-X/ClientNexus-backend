using Database.Models.Services;

namespace Database.Models.Users
{
    public enum ServiceProviderType
    {
        Lawyer,
        Other
    }
    public class ServiceProvider : BaseUser
    {
        public string Description { get; set; }
        
        // Address composite
        public int AddressId { get; set; }
        public Address Address { get; set; }
        
        // Images
        public List<string> Images { get; set; } = new();
        public string MainImage { get; set; }
        
        // Type and Status
        public ServiceProviderType Type { get; set; }
        public float Rate { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; }
        
        // Admin Approval
        public int? ApprovedById { get; set; }
        public Admin? ApprovingAdmin { get; set; }

        // Subscription composite
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
        
        public bool IsAvailableForEmergency { get; set; }

        public List<Problem>? Problems { get; set; }
        public List<Payment>? Payments { get; set; }

        public List<EmergencyCase>? EmergencyCases { get; set; }
        public List<Question>? Questions { get; set; }
        public List<ConsultationCase>? ConsultationCases { get; set; }
        public List<Appointment>? Appointments { get; set; }

        public List<SlotServiceProvider> SlotServiceProviders { get; set; }

    }
}