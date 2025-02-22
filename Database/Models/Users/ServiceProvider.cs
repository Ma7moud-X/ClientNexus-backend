using Database.Models.Others;
using Database.Models.Services;

namespace Database.Models.Users
{
    public enum SubscriptionStatus
    {
        NoSubscription = 'N',
        Active = 'A',
        Expired = 'E',
        Suspended = 'S',
        Cancelled = 'C',
    }

    public enum SubscriptionType
    {
        None = 'N',
        Free = 'F',
        Basic = 'B',
        Premium = 'P',
    }


    public class ServiceProvider : BaseUser
    {
        public string Description { get; set; } = default!;
        public string? CurrentLocation { get; set; }
        public string MainImage { get; set; } = default!;
        public float Rate { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAvailableForEmergency { get; set; }
        public int YearsOfExperience { get; set; }

        public SubscriptionType SubType { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }
        public DateTime? SubscriptionExpiryDate { get; set; }

        public int TypeId { get; set; }
        public ServiceProviderType? Type { get; set; }

        public ICollection<ServiceProviderSpecialization>? ServiceProviderSpecializations { get; set; }
        public ICollection<Specialization>? Specializations { get; set; }

        public ICollection<OfficeImageUrl>? OfficeImageUrls { get; set; }



        public int? ApprovedById { get; set; }
        public Admin? ApprovingAdmin { get; set; }

        public ICollection<License>? Licenses { get; set; }

        public ICollection<Problem>? Problems { get; set; }
        public ICollection<Service>? ServicesProvided { get; set; }
        public ICollection<SubscriptionPayment>? SubscriptionPayments { get; set; }

        public ICollection<Slot>? Slots { get; set; }

        public ICollection<ClientServiceProviderFeedback>? ClientServiceProviderFeedbacks { get; set; }
        public ICollection<Client>? ClientsWithFeedbacks { get; set; }

        public ICollection<AppointmentCost>? AppointmentCosts { get; set; }
    }
}
