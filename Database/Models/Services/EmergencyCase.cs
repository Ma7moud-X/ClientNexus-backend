using Database.Models.Users;

namespace Database.Models.Services
{
    public class EmergencyCase : Service
    {
        public string Location { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}