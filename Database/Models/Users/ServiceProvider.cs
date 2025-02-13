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
        public string? MapLocation { get; set; }
        public string MainImage { get; set; }
        public float Rate { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAvailableForEmergency { get; set; }
        public ServiceProviderType Type { get; set; }
        public List<string> Images { get; set; }
        


        public int? ApprovedById { get; set; }
        public Admin? ApprovingAdmin { get; set; }

        public int AddressId { get; set; }
        public Address Address { get; set; }

        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
        

        public List<Problem>? Problems { get; set; }
        public List<Payment>? Payments { get; set; }
        public List<EmergencyCase>? EmergencyCases { get; set; }
        public List<Question>? Questions { get; set; }
        public List<ConsultationCase>? ConsultationCases { get; set; }
        public List<Appointment>? Appointments { get; set; }


        public List<SlotServiceProvider> SlotServiceProviders { get; set; }
        public List<ClientServiceProviderFeedback> ClientServiceProviderFeedbacks { get; set; }
    }
}