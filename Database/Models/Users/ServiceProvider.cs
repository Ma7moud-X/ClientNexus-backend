using Database.Models.Others;
using Database.Models.Services;

namespace Database.Models.Users
{
    public class ServiceProvider : BaseUser
    {
        public string Description { get; set; } = default!;
        public string? MapLocation { get; set; }
        public string MainImage { get; set; } = default!;
        public float Rate { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAvailableForEmergency { get; set; }

        public int TypeId { get; set; }
        public ServiceProviderType? Type { get; set; }


        // public ICollection<string> Images { get; set; }



        public int? ApprovedById { get; set; }
        public Admin? ApprovingAdmin { get; set; }

        public Subscription? Subscription { get; set; }

        public ICollection<Address>? Addresses { get; set; }
        public ICollection<Problem>? Problems { get; set; }
        public ICollection<Service>? ServicesProvided { get; set; }
        public ICollection<SubscriptionPayment>? SubscriptionPayments { get; set; }

        public ICollection<SlotServiceProvider>? SlotServiceProviders { get; set; }

        public ICollection<ClientServiceProviderFeedback>? ClientServiceProviderFeedbacks { get; set; }
        public ICollection<Client>? ClientsWithFeedbacks { get; set; }
    }
}
