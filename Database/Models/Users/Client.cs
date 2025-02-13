using Database.Models.Services;

namespace Database.Models.Users
{
    public class Client : BaseUser
    {
        public float Rate { get; set; }
        
        public List<Problem>? Problems { get; set; }
        public List<Payment>? Payments { get; set; }
        public List<Service>? Services { get; set; }
        public List<ClientServiceProviderFeedback> ClientServiceProviderFeedbacks { get; set; }

    }
}
