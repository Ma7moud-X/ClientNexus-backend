using Database.Models.Users;

namespace Database.Models.Services
{
    public class Question : Service
    {
        public bool Visibility { get; set; }

        // Foreign key
        public int ServiceProviderId { get; set; }
        // Navigation property
        public ServiceProvider ServiceProvider { get; set; }
    }
}