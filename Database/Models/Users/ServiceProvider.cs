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
        public ICollection<string> Images { get; set; }
        


        public int? ApprovedById { get; set; }
        public Admin? ApprovingAdmin { get; set; }

        public int AddressId { get; set; }
        public Address Address { get; set; }

        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
        

        public ICollection<Problem>? Problems { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public ICollection<EmergencyCase>? EmergencyCases { get; set; }
        public ICollection<Question>? Questions { get; set; }
        public ICollection<ConsultationCase>? ConsultationCases { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }


        public ICollection<SlotServiceProvider> SlotServiceProviders { get; set; }
        public ICollection<ClientServiceProviderFeedback> ClientServiceProviderFeedbacks { get; set; }
    }
}