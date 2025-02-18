using Database.Models.Services;

namespace Database.Models.Users
{
    public class Client : BaseUser
    {
        public float Rate { get; set; }

        public ICollection<Problem>? Problems { get; set; }
        public ICollection<Service>? RequestedServices { get; set; }

        public ICollection<ClientServiceProviderFeedback>? ClientServiceProviderFeedbacks { get; set; }
        public ICollection<ServiceProvider>? FeedbackedServiceProviders { get; set; }
    }
}
