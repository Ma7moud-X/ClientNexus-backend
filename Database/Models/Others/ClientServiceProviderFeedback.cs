using Database.Models.Users;

namespace Database.Models.Services
{
    public class ClientServiceProviderFeedback
    {
        public int ClientId { get; set; }
        public int ServiceProviderId { get; set; }
        public float Rate { get; set; }
        public string Feedback {get; set;} = default!;


        public Client? Client { get; set; }
        public ServiceProvider? ServiceProvider { get; set; }
    }
}