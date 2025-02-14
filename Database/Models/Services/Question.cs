using Database.Models.Users;

namespace Database.Models.Services
{
    public class Question : Service
    {
        public bool Visibility { get; set; }

        public int ServiceProviderId { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
    }
}